package main

import (
	"context"
	"encoding/json"
	"log"
	"math/rand"
	"os"
	"strconv"
	"time"

	"github.com/google/uuid"
	"github.com/segmentio/kafka-go"
)

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

type LicensePlate struct {
	BaseNumber string `json:"BaseNumber"`
	Region     int    `json:"Region"`
}

type Vehicle struct {
	LicensePlate LicensePlate `json:"LicensePlate"`
	Make         string       `json:"Make"`
	Model        string       `json:"Model"`
}

type Fine struct {
	ID        string     `json:"Id"`
	Reason    FineReason `json:"Reason"`
	IssueDate string     `json:"IssueDate"`
	Vehicle   Vehicle    `json:"Vehicle"`
}

var (
	reasons = []FineReason{
		Speeding2040,
		Speeding4060,
		Speeding60Plus,
		RedLightViolation,
		NoParking,
		SeatbeltViolation,
		InvalidParking,
		ExpiredDocuments,
	}
	makesModels = map[string][]string{
		"Toyota":  {"Camry", "Corolla", "RAV4"},
		"Nissan":  {"Almera", "Qashqai", "X-Trail"},
		"Hyundai": {"Solaris", "Creta", "Tucson"},
		"Kia":     {"Rio", "Sportage", "Optima"},
		"Lada":    {"Vesta", "Granta", "Niva"},
	}
	letterRunes = []rune("ETOPAHKXCBM")
	numRunes    = []rune("0123456789")
)

func init() {
	rand.Seed(time.Now().UnixNano())
}

func main() {
	kafkaURL := os.Getenv("KAFKA_CONNECTION")
	minInterval, _ := strconv.Atoi(os.Getenv("MIN_INTERVAL"))
	maxInterval, _ := strconv.Atoi(os.Getenv("MAX_INTERVAL"))

	if minInterval < 1 || maxInterval < 1 || maxInterval < minInterval {
		log.Panic("Illegal arguments")
	}

	writer := kafka.NewWriter(kafka.WriterConfig{
		Brokers:  []string{kafkaURL},
		Topic:    "violations",
		Balancer: &kafka.LeastBytes{},
	})
	defer writer.Close()

	for {
		interval := rand.Intn(maxInterval-minInterval) + minInterval
		fine := generateRandomFine()

		data, err := json.Marshal(fine)
		if err != nil {
			log.Printf("Error marshaling fine: %v", err)
			continue
		}

		msg := kafka.Message{
			Key:   []byte(fine.ID),
			Value: data,
		}

		if err := writer.WriteMessages(context.Background(), msg); err != nil {
			log.Printf("Failed to write message: %v", err)
		} else {
			log.Printf("Sent fine %s to %s%d", fine.ID, fine.Vehicle.LicensePlate.BaseNumber, fine.Vehicle.LicensePlate.Region)
		}

		time.Sleep(time.Duration(interval) * time.Second)
	}
}

func generateRandomFine() Fine {
	make := randomMake()
	return Fine{
		ID:        uuid.NewString(),
		Reason:    randomReason(),
		IssueDate: time.Now().Format(time.RFC3339),
		Vehicle: Vehicle{
			LicensePlate: generateLicensePlate(),
			Make:         make,
			Model:        randomModel(make),
		},
	}
}

func generateLicensePlate() LicensePlate {
	return LicensePlate{
		BaseNumber: randomString(letterRunes, 1) +
			randomString(numRunes, 3) +
			randomString(letterRunes, 2),
		Region: rand.Intn(99) + 1,
	}
}

func randomString(runes []rune, length int) string {
	b := make([]rune, length)
	for i := range b {
		b[i] = runes[rand.Intn(len(runes))]
	}
	return string(b)
}

func randomReason() FineReason {
	return reasons[rand.Intn(len(reasons))]
}

func randomMake() string {
	makes := make([]string, 0, len(makesModels))
	for k := range makesModels {
		makes = append(makes, k)
	}
	return makes[rand.Intn(len(makes))]
}

func randomModel(make string) string {
	models := makesModels[make]
	return models[rand.Intn(len(models))]
}
