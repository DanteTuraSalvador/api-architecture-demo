import { test, expect } from '@playwright/test';

test.describe('REST API Tests', () => {
  test.describe('Vehicles API', () => {
    test('GET /api/vehicles should return OK', async ({ request }) => {
      const response = await request.get('/api/vehicles');
      expect(response.ok()).toBeTruthy();
      const vehicles = await response.json();
      expect(Array.isArray(vehicles)).toBeTruthy();
    });

    test('POST /api/vehicles should create a vehicle', async ({ request }) => {
      const newVehicle = {
        vehicleNumber: `E2E-${Date.now()}`,
        licensePlate: 'E2E-1234',
        type: 0,
        status: 0,
        mileage: 1000
      };

      const response = await request.post('/api/vehicles', {
        data: newVehicle
      });

      expect(response.status()).toBe(201);
      const vehicle = await response.json();
      expect(vehicle.vehicleNumber).toBe(newVehicle.vehicleNumber);
      expect(vehicle.id).toBeTruthy();
    });

    test('GET /api/vehicles/{id} should return vehicle when exists', async ({ request }) => {
      // First create a vehicle
      const createResponse = await request.post('/api/vehicles', {
        data: {
          vehicleNumber: `E2E-GET-${Date.now()}`,
          licensePlate: 'E2E-GET-1234',
          type: 1,
          status: 0
        }
      });
      const created = await createResponse.json();

      // Then get it by ID
      const response = await request.get(`/api/vehicles/${created.id}`);
      expect(response.ok()).toBeTruthy();
      const vehicle = await response.json();
      expect(vehicle.id).toBe(created.id);
    });

    test('GET /api/vehicles/{id} should return 404 when not found', async ({ request }) => {
      const fakeId = '00000000-0000-0000-0000-000000000000';
      const response = await request.get(`/api/vehicles/${fakeId}`);
      expect(response.status()).toBe(404);
    });

    test('PUT /api/vehicles/{id} should update vehicle', async ({ request }) => {
      // Create a vehicle
      const createResponse = await request.post('/api/vehicles', {
        data: {
          vehicleNumber: `E2E-UPDATE-${Date.now()}`,
          licensePlate: 'E2E-UPD-1234',
          type: 0,
          status: 0,
          mileage: 1000
        }
      });
      const created = await createResponse.json();

      // Update it
      const updateData = {
        vehicleNumber: created.vehicleNumber,
        licensePlate: 'E2E-UPD-9999',
        type: 1,
        status: 1,
        mileage: 2000
      };

      const response = await request.put(`/api/vehicles/${created.id}`, {
        data: updateData
      });

      expect(response.ok()).toBeTruthy();
      const updated = await response.json();
      expect(updated.mileage).toBe(2000);
    });

    test('DELETE /api/vehicles/{id} should delete vehicle', async ({ request }) => {
      // Create a vehicle
      const createResponse = await request.post('/api/vehicles', {
        data: {
          vehicleNumber: `E2E-DELETE-${Date.now()}`,
          licensePlate: 'E2E-DEL-1234',
          type: 0,
          status: 0
        }
      });
      const created = await createResponse.json();

      // Delete it
      const response = await request.delete(`/api/vehicles/${created.id}`);
      expect(response.status()).toBe(204);

      // Verify it's gone
      const getResponse = await request.get(`/api/vehicles/${created.id}`);
      expect(getResponse.status()).toBe(404);
    });

    test('GET /api/vehicles/status/{status} should filter by status', async ({ request }) => {
      const response = await request.get('/api/vehicles/status/Available');
      expect(response.ok()).toBeTruthy();
      const vehicles = await response.json();
      expect(Array.isArray(vehicles)).toBeTruthy();
    });
  });

  test.describe('Drivers API', () => {
    test('GET /api/drivers should return OK', async ({ request }) => {
      const response = await request.get('/api/drivers');
      expect(response.ok()).toBeTruthy();
      const drivers = await response.json();
      expect(Array.isArray(drivers)).toBeTruthy();
    });

    test('POST /api/drivers should create a driver', async ({ request }) => {
      const newDriver = {
        firstName: 'E2E',
        lastName: `Test-${Date.now()}`,
        licenseNumber: `DL-E2E-${Date.now()}`,
        phoneNumber: '+1-555-0000',
        email: 'e2e@test.com',
        status: 0
      };

      const response = await request.post('/api/drivers', {
        data: newDriver
      });

      expect(response.status()).toBe(201);
      const driver = await response.json();
      expect(driver.firstName).toBe(newDriver.firstName);
    });

    test('GET /api/drivers/available should return OK', async ({ request }) => {
      const response = await request.get('/api/drivers/available');
      expect(response.ok()).toBeTruthy();
    });
  });

  test.describe('Alerts API', () => {
    test('GET /api/alerts should return OK', async ({ request }) => {
      const response = await request.get('/api/alerts');
      expect(response.ok()).toBeTruthy();
      const alerts = await response.json();
      expect(Array.isArray(alerts)).toBeTruthy();
    });

    test('POST /api/alerts should create an alert', async ({ request }) => {
      const newAlert = {
        vehicleId: '00000000-0000-0000-0000-000000000001',
        type: 0,
        level: 1,
        message: 'E2E Test Alert'
      };

      const response = await request.post('/api/alerts', {
        data: newAlert
      });

      expect(response.status()).toBe(201);
      const alert = await response.json();
      expect(alert.message).toBe(newAlert.message);
      expect(alert.isAcknowledged).toBe(false);
    });

    test('GET /api/alerts/unacknowledged should return OK', async ({ request }) => {
      const response = await request.get('/api/alerts/unacknowledged');
      expect(response.ok()).toBeTruthy();
    });

    test('GET /api/alerts/recent should return OK', async ({ request }) => {
      const response = await request.get('/api/alerts/recent?count=5');
      expect(response.ok()).toBeTruthy();
    });
  });

  test.describe('Trips API', () => {
    test('GET /api/trips should return OK', async ({ request }) => {
      const response = await request.get('/api/trips');
      expect(response.ok()).toBeTruthy();
      const trips = await response.json();
      expect(Array.isArray(trips)).toBeTruthy();
    });

    test('GET /api/trips/active should return OK', async ({ request }) => {
      const response = await request.get('/api/trips/active');
      expect(response.ok()).toBeTruthy();
    });
  });
});

test.describe('GraphQL API Tests', () => {
  test('should execute vehicles query', async ({ request }) => {
    const response = await request.post('/graphql', {
      data: {
        query: `
          query {
            vehicles {
              id
              vehicleNumber
              status
            }
          }
        `
      }
    });

    expect(response.ok()).toBeTruthy();
    const result = await response.json();
    expect(result.data).toBeTruthy();
    expect(result.data.vehicles).toBeTruthy();
    expect(Array.isArray(result.data.vehicles)).toBeTruthy();
  });

  test('should execute drivers query', async ({ request }) => {
    const response = await request.post('/graphql', {
      data: {
        query: `
          query {
            drivers {
              id
              firstName
              lastName
              status
            }
          }
        `
      }
    });

    expect(response.ok()).toBeTruthy();
    const result = await response.json();
    expect(result.data.drivers).toBeTruthy();
  });

  test('should execute alerts query with filter', async ({ request }) => {
    const response = await request.post('/graphql', {
      data: {
        query: `
          query {
            alerts(unacknowledgedOnly: true) {
              id
              type
              level
              message
              isAcknowledged
            }
          }
        `
      }
    });

    expect(response.ok()).toBeTruthy();
    const result = await response.json();
    expect(result.data.alerts).toBeTruthy();
  });

  test('should execute fleets query', async ({ request }) => {
    const response = await request.post('/graphql', {
      data: {
        query: `
          query {
            fleets {
              id
              name
              description
            }
          }
        `
      }
    });

    expect(response.ok()).toBeTruthy();
    const result = await response.json();
    expect(result.data.fleets).toBeTruthy();
  });
});

test.describe('SSE API Tests', () => {
  test('GET /sse/stats should return connection statistics', async ({ request }) => {
    const response = await request.get('/sse/stats');
    expect(response.ok()).toBeTruthy();
    const stats = await response.json();
    expect(stats.connectedClients).toBeDefined();
    expect(stats.timestamp).toBeDefined();
  });

  test('POST /sse/alerts/test should trigger a test alert', async ({ request }) => {
    const response = await request.post('/sse/alerts/test', {
      data: {
        vehicleId: '00000000-0000-0000-0000-000000000001',
        type: 0,
        level: 1,
        message: 'E2E Test SSE Alert'
      }
    });

    expect(response.ok()).toBeTruthy();
    const result = await response.json();
    expect(result.message).toBe('Alert triggered and broadcast');
    expect(result.alert).toBeTruthy();
  });
});

test.describe('Swagger/OpenAPI Tests', () => {
  test('Swagger UI should be accessible', async ({ page }) => {
    await page.goto('/');
    await expect(page).toHaveTitle(/Swagger/i);
  });

  test('Swagger JSON should be available', async ({ request }) => {
    const response = await request.get('/swagger/v1/swagger.json');
    expect(response.ok()).toBeTruthy();
    const swagger = await response.json();
    expect(swagger.openapi).toBeTruthy();
    expect(swagger.info.title).toBe('Fleet Management API');
  });
});
