package main

import (
	"bytes"
	"database/sql"
	"encoding/json"
	"flag"
	"fmt"
	"log"
	"math/rand"
	"net/http"
	"time"

	"github.com/IBM/sarama"
	_ "github.com/lib/pq"
)

type Car struct {
	BaseNumber string `json:"baseNumber"`
	Region     int    `json:"region"`
}

type Plate struct {
	BaseNumber string `json:"baseNumber"`
	Region     int    `json:"region"`
}

type FineRequest struct {
	Plate     Plate  `json:"plate"`
	Reason    string `json:"reason"`
	IssueDate string `json:"issueDate"`
}

// FineReason enum-like type
type FineReason string

const (
	Speeding2040      FineReason = "speeding (20-40 km/h)"
	Speeding4060      FineReason = "speeding (40-60 km/h)"
	Speeding60Plus    FineReason = "speeding (60+ km/h)"
	RedLightViolation FineReason = "running a red light"
	NoParking         FineReason = "parking in a no-parking zone"
	SeatbeltViolation FineReason = "seatbelt violation"
	InvalidParking    FineReason = "invalid parking"
	ExpiredDocuments  FineReason = "expired registration or insurance"
)

var allFineReasons = []FineReason{
	Speeding2040,
	Speeding4060,
	Speeding60Plus,
	RedLightViolation,
	NoParking,
	SeatbeltViolation,
	InvalidParking,
	ExpiredDocuments,
}

func randomFineReason() FineReason {
	return allFineReasons[rand.Intn(len(allFineReasons))]
}

var (
	minDelay       = flag.Int("min_delay", 30, "Minimum delay between fines in seconds")
	maxDelay       = flag.Int("max_delay", 180, "Maximum delay between fines in seconds")
	dbConnStr      = flag.String("db_conn", "", "Database connection string")
	kafkaConnStr   = flag.String("kafka_conn", "", "Kafka connection string")
	fineServiceURL = flag.String("fine_service_url", "", "URL of the fine processing service")
)

func main() {
	flag.Parse()

	// Validate required flags
	if *dbConnStr == "" || *kafkaConnStr == "" || *fineServiceURL == "" {
		log.Fatal("Missing required flags: db_conn, kafka_conn and fine_service_url must be provided")
	}

	// Validate delay values
	if *minDelay <= 0 || *maxDelay <= 0 || *minDelay > *maxDelay {
		log.Fatal("Invalid delay values: min_delay must be <= max_delay and both must be positive")
	}

	// Initialize database connection
	db, err := sql.Open("postgres", *dbConnStr)
	if err != nil {
		log.Fatal("Failed to connect to database:", err)
	}
	defer db.Close()

	err = db.Ping()
	if err != nil {
		log.Fatal("Database ping failed:", err)
	}

	// Create cars table if not exists
	_, err = db.Exec(`CREATE TABLE IF NOT EXISTS cars (
		base_number TEXT NOT NULL,
		region INTEGER NOT NULL,
		PRIMARY KEY (base_number, region)
	)`)
	if err != nil {
		log.Fatal("Failed to create table:", err)
	}

	// Initialize Kafka consumer
	config := sarama.NewConfig()
	config.Consumer.Return.Errors = true

	consumer, err := sarama.NewConsumer([]string{*kafkaConnStr}, config)
	if err != nil {
		log.Fatal("Failed to create Kafka consumer:", err)
	}
	defer consumer.Close()

	partitionConsumer, err := consumer.ConsumePartition("registered-cars", 0, sarama.OffsetNewest)
	if err != nil {
		log.Fatal("Failed to start partition consumer:", err)
	}
	defer partitionConsumer.Close()

	// Start Kafka consumer goroutine
	go func() {
		for {
			select {
			case msg := <-partitionConsumer.Messages():
				var car Car
				if err := json.Unmarshal(msg.Value, &car); err != nil {
					log.Printf("Failed to unmarshal car: %v", err)
					continue
				}

				_, err := db.Exec(
					"INSERT INTO cars (base_number, region) VALUES ($1, $2) ON CONFLICT (base_number, region) DO NOTHING",
					car.BaseNumber,
					car.Region,
				)
				if err != nil {
					log.Printf("Failed to insert car: %v", err)
				}

			case err := <-partitionConsumer.Errors():
				log.Printf("Kafka consumer error: %v", err)
			}
		}
	}()

	// Start fine issuance loop with random delays
	for {
		delay := time.Duration(rand.Intn(*maxDelay-*minDelay+1)+*minDelay) * time.Second
		time.Sleep(delay)

		var car Car
		err := db.QueryRow(
			"SELECT base_number, region FROM cars ORDER BY RANDOM() LIMIT 1",
		).Scan(&car.BaseNumber, &car.Region)

		if err != nil {
			if err == sql.ErrNoRows {
				log.Println("No cars available for fine issuance")
				continue
			}
			log.Printf("Failed to select random car: %v", err)
			continue
		}

		fine := FineRequest{
			Plate: Plate{
				BaseNumber: car.BaseNumber,
				Region:     car.Region,
			},
			Reason:    string(randomFineReason()),
			IssueDate: time.Now().Format(time.RFC3339),
		}

		body, err := json.Marshal(fine)
		if err != nil {
			log.Printf("Failed to marshal fine: %v", err)
			continue
		}

		resp, err := http.Post(
			fmt.Sprintf("%s/api/fines", *fineServiceURL),
			"application/json",
			bytes.NewReader(body),
		)
		if err != nil {
			log.Printf("Failed to send fine: %v", err)
			continue
		}
		defer resp.Body.Close()

		if resp.StatusCode != http.StatusOK {
			log.Printf("Unexpected response status: %d", resp.StatusCode)
		} else {
			log.Printf("Successfully sent fine for %s-%d: %s", car.BaseNumber, car.Region, fine.Reason)
		}
	}
}

func init() {
	rand.Seed(time.Now().UnixNano())
}
