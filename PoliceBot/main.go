package main

import (
	"encoding/json"
	"flag"
	"io"
	"log"
	"net/http"
	"time"
)

type Fine struct {
	ID string `json:"Id"`
}

func main() {
	port := flag.String("port", "8080", "Port to listen on")
	baseURL := flag.String("baseUrl", "", "Base URL for forwarding POST requests")
	flag.Parse()

	if *baseURL == "" {
		log.Fatal("baseUrl parameter is required")
	}

	client := &http.Client{Timeout: 15 * time.Second}

	http.HandleFunc("/notifications", func(w http.ResponseWriter, r *http.Request) {
		if r.Method != http.MethodPost {
			http.Error(w, "Only POST allowed", http.StatusMethodNotAllowed)
			return
		}

		body, err := io.ReadAll(r.Body)
		if err != nil {
			http.Error(w, "Failed to read body", http.StatusBadRequest)
			return
		}
		defer r.Body.Close()

		var fine Fine
		if err := json.Unmarshal(body, &fine); err != nil {
			http.Error(w, "Invalid JSON", http.StatusBadRequest)
			return
		}
		if fine.ID == "" {
			http.Error(w, "Missing fine id", http.StatusBadRequest)
			return
		}

		w.WriteHeader(http.StatusOK)

		go func() {
			time.Sleep(20 * time.Second) // пользователь проверяет корректность штрафа
			url := *baseURL + "/" + fine.ID

			req, err := http.NewRequest(http.MethodPost, url, nil)
			if err != nil {
				log.Printf("Failed to create POST request: %v", err)
				return
			}

			resp, err := client.Do(req)
			if err != nil {
				log.Printf("POST request to %s failed: %v", url, err)
				return
			}
			resp.Body.Close()
			log.Printf("POST request to %s completed with status %s", url, resp.Status)
		}()
	})

	log.Printf("Police service listening on :%s, forwarding to baseUrl=%s", *port, *baseURL)
	if err := http.ListenAndServe(":"+*port, nil); err != nil {
		log.Fatalf("Server failed: %v", err)
	}
}
