package main

import (
	"context"
	"database/sql"
	"encoding/json"
	"flag"
	"fmt"
	"log"
	"net/http"

	"github.com/google/uuid"
	"github.com/jackc/pgx/v5/pgxpool"
)

var connectionString = flag.String("conn", "", "Database connection string")

type FineInfo struct {
	Id          uuid.UUID `json:"id"`
	Price       float64   `json:"price"`
	BankCode    uint64    `json:"bankCode"`
	AccountCode uint64    `json:"accountCode"`
}

func fetchFineInfo(db *pgxpool.Pool, reason string, _ string) (*FineInfo, error) {
	var id uuid.UUID
	var price float64
	var bankCode uint64
	var accountCode uint64
	ctx := context.Background()

	id, _ = uuid.NewV7()
	query := "SELECT cost FROM public.fines WHERE reason = $1"
	err := db.QueryRow(ctx, query, reason).Scan(&price)
	if err == sql.ErrNoRows {
		return nil, fmt.Errorf("штраф не найден")
	} else if err != nil {
		return nil, fmt.Errorf("ошибка чтения базы данных: %v", err)
	}

	query = "SELECT bank_code, account_code FROM public.payment WHERE id=1"
	err = db.QueryRow(ctx, query).Scan(&bankCode, &accountCode)

	if err == sql.ErrNoRows {
		return nil, fmt.Errorf("отсутствуют реквизиты")
	} else if err != nil {
		return nil, fmt.Errorf("ошибка чтения базы данных: %v", err)
	}
	info := &FineInfo{
		Id:          id,
		Price:       price,
		BankCode:    bankCode,
		AccountCode: accountCode,
	}

	return info, nil
}

func fineHandler(w http.ResponseWriter, r *http.Request, db *pgxpool.Pool) {

	var input struct {
		Reason    string `json:"reason"`
		IssueDate string `json:"issueDate"`
	}

	err := json.NewDecoder(r.Body).Decode(&input)
	if err != nil {
		http.Error(w, "Некорректные данные JSON", http.StatusBadRequest)
		return
	}

	fineInfo, err := fetchFineInfo(db, input.Reason, input.IssueDate)
	if err != nil {
		log.Printf("Ошибка обработки штрафа: %v\n", err)
		http.Error(w, err.Error(), http.StatusNotFound)
		return
	}

	w.Header().Set("Content-Type", "application/json")
	json.NewEncoder(w).Encode(fineInfo)
}

func main() {
	flag.Parse()

	config, err := pgxpool.ParseConfig(*connectionString)
	if err != nil {
		log.Fatalf("Ошибка парсинга connection string: %v", err)
	}

	dbpool, err := pgxpool.NewWithConfig(context.Background(), config)

	if err != nil {
		log.Fatalf("Ошибка открытия соединения с базой данных: %v", err)
	}
	defer dbpool.Close()

	router := http.NewServeMux()
	router.HandleFunc("/fine", func(w http.ResponseWriter, r *http.Request) {
		fineHandler(w, r, dbpool)
	})

	fmt.Println("Запуск сервера на порту :8080...")
	log.Fatal(http.ListenAndServe(":8080", router))
}
