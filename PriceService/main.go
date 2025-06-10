package main

import (
	"encoding/json"
	"flag"
	"log"
	"net/http"

	"github.com/google/uuid"
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

var BankCode *int64
var AccountCode *int64

type FineRequest struct {
	Reason    FineReason `json:"Reason"`
	IssueDate string     `json:"IssueDate"`
}

type FineResponse struct {
	Id          string  `json:"Id"`
	Price       float64 `json:"Price"`
	BankCode    int64   `json:"BankCode"`
	AccountCode int64   `json:"AccountCode"`
}

var finePrices = map[FineReason]float64{
	Speeding2040:      750,
	Speeding4060:      1500,
	Speeding60Plus:    3000,
	RedLightViolation: 1500,
	NoParking:         1500,
	SeatbeltViolation: 1500,
	InvalidParking:    3000,
	ExpiredDocuments:  850,
}

func fineHandler(w http.ResponseWriter, r *http.Request) {
	if r.Method != http.MethodPost {
		http.Error(w, "Only POST method is allowed", http.StatusMethodNotAllowed)
		return
	}

	var req FineRequest
	if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
		http.Error(w, "Invalid JSON body", http.StatusBadRequest)
		return
	}

	price, ok := finePrices[req.Reason]
	if !ok {
		http.Error(w, "Unknown Reason", http.StatusBadRequest)
		return
	}

	resp := FineResponse{
		Id:          uuid.NewString(),
		Price:       price,
		BankCode:    *BankCode,
		AccountCode: *AccountCode,
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(resp)
}

func main() {
	port := flag.String("port", "8080", "Port to listen on")
	BankCode = flag.Int64("bank", 1234567, "Bank code")
	AccountCode = flag.Int64("account", 12345, "Account code")
	flag.Parse()
	http.HandleFunc("/fine", fineHandler)
	log.Printf("Server started at :%s", *port)
	log.Fatal(http.ListenAndServe(":"+*port, nil))
}
