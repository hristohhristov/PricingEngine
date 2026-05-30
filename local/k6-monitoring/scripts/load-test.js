import http from 'k6/http';
import { check } from 'k6';

export const options = {
    scenarios: {
        constant_request_rate: {
            executor: 'constant-arrival-rate',
            rate: 167,                // Target: 167 requests
            timeUnit: '1m',           // distributed evenly over exactly 1 minute (~2.78 RPS)
            duration: '1m',           // Total test duration: exactly 1 minute
            preAllocatedVUs: 5,       // Initial Virtual Users
            maxVUs: 20,               // Maximum scale-up limit
        },
    },
    thresholds: {
        http_req_failed: ['rate<0.01'],   // Test fails if more than 1% of requests error out
        http_req_duration: ['p(95)<500'], // 95% of requests must respond in less than 500ms
    },
};

export default function () {
    // Target endpoint matching your .NET API running inside Docker on port 8080
    const url = 'http://host.docker.internal:8080/api/v1/quotes'; 
    
    // Define the request body based on your CalculateQuoteRequest schema.
    // Contains the specific product code and the inner insurance risk payload.
    const requestBody = {
        productCode: "AUTO_V1",
        payload: {
            vehicleValue: 25000,
            driverAge: 22,
            coverageType: "Comprehensive",
            comprehensiveRider: true
        }
    };

    // Serialize the JavaScript object into a JSON string (payload)
    const payload = JSON.stringify(requestBody);

    // Configuration headers telling the .NET API to bind this to [FromBody] CalculateQuoteRequest
    const params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    // Execute the actual HTTP POST load test request with the payload
    const res = http.post(url, payload, params);

    // Assert that the API successfully processed the calculation (HTTP 200 or 201)
    check(res, {
        'status is 200 or 201': (r) => r.status === 200 || r.status === 201,
    });
}