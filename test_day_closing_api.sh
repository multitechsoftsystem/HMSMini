#!/bin/bash

# Get token
echo "Getting authentication token..."
TOKEN=$(curl -s -X POST http://localhost:5096/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin@123"}' | grep -o '"token":"[^"]*' | cut -d'"' -f4)

echo "Token obtained"
echo ""

echo "=============================================="
echo "Day Closing API Endpoint Tests"
echo "=============================================="
echo ""

echo "1. GET /api/day-closing/working-date"
echo "----------------------------------------------"
curl -s -X GET "http://localhost:5096/api/day-closing/working-date" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
echo ""
echo ""

echo "2. GET /api/day-closing/validate"
echo "----------------------------------------------"
curl -s -X GET "http://localhost:5096/api/day-closing/validate" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
echo ""
echo ""

echo "3. GET /api/day-closing/preview"
echo "----------------------------------------------"
curl -s -X GET "http://localhost:5096/api/day-closing/preview" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
echo ""
echo ""

echo "4. GET /api/day-closing/history"
echo "----------------------------------------------"
curl -s -X GET "http://localhost:5096/api/day-closing/history?pageSize=5" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json"
echo ""
echo ""

echo "5. POST /api/day-closing/close (DRY RUN - Not executing)"
echo "----------------------------------------------"
echo "Skipping actual day close execution for safety"
echo ""

echo "=============================================="
echo "All tests completed"
echo "=============================================="
