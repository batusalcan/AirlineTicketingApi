import http from "k6/http";
import { check, sleep } from "k6";

// -------------------------------------------------------------
// 3-STAGE LOAD TESTING SCENARIO (Total 35 Seconds)
// -------------------------------------------------------------
export const options = {
  stages: [
    { duration: "10s", target: 20 }, // Stage 1: Normal Load (20 Virtual Users)
    { duration: "10s", target: 50 }, // Stage 2: Peak Load (50 Virtual Users)
    { duration: "15s", target: 100 }, // Stage 3: Stress Load (100 Virtual Users)
  ],
};

// -------------------------------------------------------------
// SETUP PHASE
// Executes once before the load test starts.
// Authenticates as an admin and retrieves the JWT token.
// -------------------------------------------------------------
export function setup() {
  const loginUrl = "http://localhost:5055/gateway/v1/auth/login";
  const payload = JSON.stringify({ username: "admin", password: "admin123" });
  const params = { headers: { "Content-Type": "application/json" } };

  const res = http.post(loginUrl, payload, params);

  // Extract and return the token (.NET JSON serialization uses camelCase)
  return { token: res.json("token") };
}

// -------------------------------------------------------------
// MAIN EXECUTION LOOP
// The core operations executed by virtual users in a continuous loop.
// -------------------------------------------------------------
export default function (data) {
  // =============================================================
  // ENDPOINT 1: BUY TICKET (Concurrency & Race Condition Evaluation)
  // =============================================================
  const buyUrl = "http://localhost:5055/gateway/v1/ticket/buy";
  const buyPayload = JSON.stringify({
    // Target flight intentionally set with a low capacity (e.g., 15) to trigger concurrency checks
    flightNumber: "TEST-100",
    date: "2026-05-01T00:00:00Z",
    // Generates a unique passenger name per virtual user and iteration
    passengerName: `LoadTest_User_${__VU}_${__ITER}`,
  });

  const buyParams = {
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${data.token}`,
    },
  };

  const buyRes = http.post(buyUrl, buyPayload, buyParams);

  // Categorize responses to evaluate the error rate and system resilience
  check(buyRes, {
    "Buy Ticket: SUCCESS (Ticket Purchased)": (r) => r.status === 200,
    "Buy Ticket: FAILED (Sold Out or Race Condition Blocked)": (r) =>
      r.status === 400,
  });

  // =============================================================
  // ENDPOINT 2: CHECK-IN (Unauthenticated Load Test)
  // =============================================================
  const checkinUrl = "http://localhost:5055/gateway/v1/ticket/checkin";
  const checkinPayload = JSON.stringify({
    flightNumber: "TEST-100",
    date: "2026-05-01T00:00:00Z",
    passengerName: `LoadTest_User_${__VU}_${__ITER}`,
  });

  const checkinParams = { headers: { "Content-Type": "application/json" } };
  const checkinRes = http.post(checkinUrl, checkinPayload, checkinParams);

  check(checkinRes, {
    "Check-In: PROCESSED": (r) => r.status === 200 || r.status === 400,
  });

  // Simulate user think time: Pause for 1 second after completing both operations
  sleep(1);
}
