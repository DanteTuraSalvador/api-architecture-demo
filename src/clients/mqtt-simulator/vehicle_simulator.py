#!/usr/bin/env python3
"""
Vehicle MQTT Simulator - Simulates a single vehicle sending telemetry data
Demonstrates MQTT pub/sub messaging for IoT scenarios
"""

import json
import random
import time
import argparse
from datetime import datetime, timezone
import paho.mqtt.client as mqtt


class VehicleSimulator:
    def __init__(self, broker_host: str, broker_port: int, fleet_id: str, vehicle_id: str):
        self.broker_host = broker_host
        self.broker_port = broker_port
        self.fleet_id = fleet_id
        self.vehicle_id = vehicle_id
        self.client = mqtt.Client(mqtt.CallbackAPIVersion.VERSION2)

        # Initial vehicle state
        self.latitude = 37.7749 + random.uniform(-0.1, 0.1)  # San Francisco area
        self.longitude = -122.4194 + random.uniform(-0.1, 0.1)
        self.speed = 0.0
        self.heading = random.uniform(0, 360)
        self.fuel_level = random.uniform(50, 100)
        self.engine_temp = random.uniform(180, 210)
        self.battery_voltage = random.uniform(12.0, 14.5)
        self.odometer = random.uniform(10000, 100000)

        # Set up callbacks
        self.client.on_connect = self._on_connect
        self.client.on_disconnect = self._on_disconnect

    def _on_connect(self, client, userdata, flags, rc, properties):
        if rc == 0:
            print(f"[{self.vehicle_id}] Connected to MQTT broker")
        else:
            print(f"[{self.vehicle_id}] Connection failed with code {rc}")

    def _on_disconnect(self, client, userdata, flags, rc, properties):
        print(f"[{self.vehicle_id}] Disconnected from broker")

    def connect(self):
        """Connect to the MQTT broker"""
        try:
            self.client.connect(self.broker_host, self.broker_port, 60)
            self.client.loop_start()
            return True
        except Exception as e:
            print(f"[{self.vehicle_id}] Connection error: {e}")
            return False

    def disconnect(self):
        """Disconnect from the MQTT broker"""
        self.client.loop_stop()
        self.client.disconnect()

    def _get_topic(self, message_type: str) -> str:
        """Generate MQTT topic for this vehicle"""
        return f"fleet/{self.fleet_id}/vehicle/{self.vehicle_id}/{message_type}"

    def _update_simulation(self):
        """Update simulated vehicle state"""
        # Random movement
        self.latitude += random.uniform(-0.001, 0.001)
        self.longitude += random.uniform(-0.001, 0.001)

        # Speed changes
        speed_delta = random.uniform(-5, 5)
        self.speed = max(0, min(80, self.speed + speed_delta))

        # Heading drift
        self.heading = (self.heading + random.uniform(-10, 10)) % 360

        # Fuel consumption
        if self.speed > 0:
            self.fuel_level = max(0, self.fuel_level - random.uniform(0.01, 0.05))

        # Engine temperature fluctuation
        self.engine_temp += random.uniform(-2, 2)
        self.engine_temp = max(170, min(230, self.engine_temp))

        # Battery voltage
        self.battery_voltage += random.uniform(-0.1, 0.1)
        self.battery_voltage = max(11.5, min(14.8, self.battery_voltage))

        # Odometer
        self.odometer += self.speed / 3600  # Convert mph to miles per second

    def publish_location(self):
        """Publish current location"""
        payload = {
            "latitude": round(self.latitude, 6),
            "longitude": round(self.longitude, 6),
            "speed": round(self.speed, 1),
            "heading": round(self.heading, 1),
            "timestamp": datetime.now(timezone.utc).isoformat()
        }
        topic = self._get_topic("location")
        self.client.publish(topic, json.dumps(payload))
        print(f"[{self.vehicle_id}] Location: {payload['latitude']}, {payload['longitude']} @ {payload['speed']} mph")

    def publish_telemetry(self):
        """Publish telemetry data"""
        payload = {
            "fuel_level": round(self.fuel_level, 1),
            "engine_temp": round(self.engine_temp, 1),
            "battery_voltage": round(self.battery_voltage, 2),
            "odometer": round(self.odometer, 1),
            "timestamp": datetime.now(timezone.utc).isoformat()
        }
        topic = self._get_topic("telemetry")
        self.client.publish(topic, json.dumps(payload))
        print(f"[{self.vehicle_id}] Telemetry: Fuel={payload['fuel_level']}%, Temp={payload['engine_temp']}F")

    def publish_status(self, status: str):
        """Publish vehicle status"""
        payload = {
            "status": status,
            "timestamp": datetime.now(timezone.utc).isoformat()
        }
        topic = self._get_topic("status")
        self.client.publish(topic, json.dumps(payload))
        print(f"[{self.vehicle_id}] Status: {status}")

    def publish_alert(self, alert_type: str, message: str):
        """Publish an alert"""
        payload = {
            "type": alert_type,
            "message": message,
            "timestamp": datetime.now(timezone.utc).isoformat()
        }
        topic = self._get_topic("alert")
        self.client.publish(topic, json.dumps(payload))
        print(f"[{self.vehicle_id}] ALERT: {alert_type} - {message}")

    def check_for_alerts(self):
        """Check conditions and generate alerts"""
        if self.fuel_level < 15:
            self.publish_alert("LOW_FUEL", f"Fuel level critical: {self.fuel_level:.1f}%")
        if self.engine_temp > 220:
            self.publish_alert("HIGH_TEMP", f"Engine overheating: {self.engine_temp:.1f}F")
        if self.battery_voltage < 11.8:
            self.publish_alert("LOW_BATTERY", f"Battery voltage low: {self.battery_voltage:.2f}V")

    def run(self, interval: float = 1.0):
        """Run the simulation loop"""
        if not self.connect():
            return

        self.publish_status("ACTIVE")

        try:
            telemetry_counter = 0
            while True:
                self._update_simulation()

                # Publish location every interval
                self.publish_location()

                # Publish telemetry less frequently
                telemetry_counter += 1
                if telemetry_counter >= 5:
                    self.publish_telemetry()
                    self.check_for_alerts()
                    telemetry_counter = 0

                time.sleep(interval)

        except KeyboardInterrupt:
            print(f"\n[{self.vehicle_id}] Stopping simulation...")
            self.publish_status("OFFLINE")
        finally:
            self.disconnect()


def main():
    parser = argparse.ArgumentParser(description="Vehicle MQTT Simulator")
    parser.add_argument("--host", default="localhost", help="MQTT broker host")
    parser.add_argument("--port", type=int, default=1883, help="MQTT broker port")
    parser.add_argument("--fleet", default="fleet-001", help="Fleet ID")
    parser.add_argument("--vehicle", default="vehicle-001", help="Vehicle ID")
    parser.add_argument("--interval", type=float, default=1.0, help="Update interval in seconds")

    args = parser.parse_args()

    print(f"Starting vehicle simulator: {args.vehicle} in fleet {args.fleet}")
    print(f"Connecting to {args.host}:{args.port}")

    simulator = VehicleSimulator(args.host, args.port, args.fleet, args.vehicle)
    simulator.run(args.interval)


if __name__ == "__main__":
    main()
