package main

import (
	"bytes"
	"context"
	"encoding/json"
	"flag"
	"log"
	"net/http"
	"time"

	"github.com/segmentio/kafka-go"
)

type Violation struct {
	ID        string `json:"Id"`
	Reason    string `json:"Reason"`
	IssueDate string `json:"IssueDate"`
}

type FineStatus string

const (
	Suspected FineStatus = "Suspected"
	Confirmed FineStatus = "Confirmed"
	Rejected  FineStatus = "Rejected"
	Paid      FineStatus = "Paid"
)

type Fine struct {
	ID        string     `json:"Id"`
	Reason    string     `json:"Reason"`
	IssueDate string     `json:"IssueDate"`
	Status    FineStatus `json:"Status"`
}

type ConfiscationOrder struct {
	ID           string `json:"Id"`
	LicensePlate struct {
		BaseNumber string `json:"BaseNumber"`
		Region     int    `json:"Region"`
	} `json:"LicensePlate"`
	IsReturned bool `json:"IsReturned"`
}

func main() {
	kafkaConn := flag.String("kafka_connection", "localhost:9092", "Kafka connection string")
	policeAddr := flag.String("policeAddress", "http://localhost:8080/notifications", "Police API endpoint")
	flag.Parse()

	go gg(*kafkaConn, *policeAddr)

	go confiscationNotifications(*kafkaConn)

	finesNotifications(*kafkaConn)
}

func gg(kafkaConn, policeAddr string) {
	reader := kafka.NewReader(kafka.ReaderConfig{
		Brokers: []string{kafkaConn},
		Topic:   "violations",
		GroupID: "NotificationService",
	})
	defer reader.Close()

	client := &http.Client{Timeout: 5 * time.Second}

	for {
		msg, err := reader.ReadMessage(context.Background())
		if err != nil {
			log.Printf("Kafka read error: %v", err)
			continue
		}

		var fine Violation
		if err := json.Unmarshal(msg.Value, &fine); err != nil {
			log.Printf("JSON unmarshal error: %v", err)
			continue
		}

		payload, _ := json.Marshal(fine)
		resp, err := client.Post(policeAddr, "application/json", bytes.NewReader(payload))
		if err != nil {
			log.Printf("API request failed: %v", err)
			continue
		}
		resp.Body.Close()

		if resp.StatusCode >= 400 {
			log.Printf("API responded with error: %s", resp.Status)
		} else {
			log.Printf("Зафиксировано правонарушение ID: %s. Полиция уведомлена", fine.ID)
		}
	}
}

func confiscationNotifications(kafkaConn string) {
	reader := kafka.NewReader(kafka.ReaderConfig{
		Brokers: []string{kafkaConn},
		Topic:   "confiscation",
		GroupID: "NotificationService",
	})
	defer reader.Close()

	var order ConfiscationOrder

	for {
		msg, err := reader.ReadMessage(context.Background())
		if err != nil {
			log.Printf("Error reading confiscation message: %v", err)
			continue
		}

		if err := json.Unmarshal(msg.Value, &order); err != nil {
			log.Printf("JSON unmarshal error: %v", err)
			continue
		}

		if order.IsReturned {
			log.Printf("Автомобиль %s%d больше не на штрафстоянке", order.LicensePlate.BaseNumber,
				order.LicensePlate.Region)
		} else {
			log.Printf("Автомобиль %s%d конфискован. Владелец уведомлён", order.LicensePlate.BaseNumber,
				order.LicensePlate.Region)
		}
	}
}

func finesNotifications(kafkaConn string) {
	reader := kafka.NewReader(kafka.ReaderConfig{
		Brokers: []string{kafkaConn},
		Topic:   "fines",
		GroupID: "NotificationService",
	})
	defer reader.Close()

	var fine Fine

	for {
		msg, err := reader.ReadMessage(context.Background())
		if err != nil {
			log.Printf("Error reading confiscation message: %v", err)
			continue
		}

		if err := json.Unmarshal(msg.Value, &fine); err != nil {
			log.Printf("JSON unmarshal error: %v", err)
			continue
		}

		switch fine.Status {
		case Confirmed:
			log.Printf("Выписан штраф ID: %s. Пользователь уведомлён", fine.ID)
		case Rejected:
			log.Printf("Обжалован штраф ID: %s. Пользователь уведомлён", fine.ID)
		case Paid:
			log.Printf("Оплата штрафа ID: %s прошла успешно. Пользователь уведомлён", fine.ID)
		}
	}
}
