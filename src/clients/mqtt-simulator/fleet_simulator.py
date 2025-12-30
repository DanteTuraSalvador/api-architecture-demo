#!/usr/bin/env python3
"""
Fleet MQTT Simulator - Simulates multiple vehicles in a fleet
Demonstrates MQTT pub/sub messaging for IoT fleet management
"""

import argparse
import threading
import time
import signal
import sys
from vehicle_simulator import VehicleSimulator


class FleetSimulator:
    def __init__(self, broker_host: str, broker_port: int, fleet_id: str, vehicle_count: int):
        self.broker_host = broker_host
        self.broker_port = broker_port
        self.fleet_id = fleet_id
        self.vehicle_count = vehicle_count
        self.vehicles: list[VehicleSimulator] = []
        self.threads: list[threading.Thread] = []
        self.running = False

    def start(self, interval: float = 1.0):
        """Start simulating all vehicles"""
        self.running = True

        print(f"Starting fleet simulation with {self.vehicle_count} vehicles...")

        for i in range(self.vehicle_count):
            vehicle_id = f"vehicle-{i+1:03d}"
            simulator = VehicleSimulator(
                self.broker_host,
                self.broker_port,
                self.fleet_id,
                vehicle_id
            )
            self.vehicles.append(simulator)

            # Create a thread for each vehicle
            thread = threading.Thread(
                target=self._run_vehicle,
                args=(simulator, interval),
                daemon=True
            )
            self.threads.append(thread)
            thread.start()

            # Stagger vehicle starts
            time.sleep(0.1)

        print(f"All {self.vehicle_count} vehicles started")

    def _run_vehicle(self, simulator: VehicleSimulator, interval: float):
        """Run a single vehicle simulation"""
        if not simulator.connect():
            return

        simulator.publish_status("ACTIVE")

        try:
            telemetry_counter = 0
            while self.running:
                simulator._update_simulation()
                simulator.publish_location()

                telemetry_counter += 1
                if telemetry_counter >= 5:
                    simulator.publish_telemetry()
                    simulator.check_for_alerts()
                    telemetry_counter = 0

                time.sleep(interval)
        except Exception as e:
            print(f"[{simulator.vehicle_id}] Error: {e}")
        finally:
            simulator.publish_status("OFFLINE")
            simulator.disconnect()

    def stop(self):
        """Stop all vehicle simulations"""
        print("\nStopping fleet simulation...")
        self.running = False

        # Wait for all threads to finish
        for thread in self.threads:
            thread.join(timeout=2.0)

        print("Fleet simulation stopped")


def main():
    parser = argparse.ArgumentParser(description="Fleet MQTT Simulator")
    parser.add_argument("--host", default="localhost", help="MQTT broker host")
    parser.add_argument("--port", type=int, default=1883, help="MQTT broker port")
    parser.add_argument("--fleet", default="fleet-001", help="Fleet ID")
    parser.add_argument("--count", type=int, default=5, help="Number of vehicles to simulate")
    parser.add_argument("--interval", type=float, default=1.0, help="Update interval in seconds")

    args = parser.parse_args()

    print(f"Fleet MQTT Simulator")
    print(f"====================")
    print(f"Broker: {args.host}:{args.port}")
    print(f"Fleet ID: {args.fleet}")
    print(f"Vehicle Count: {args.count}")
    print(f"Update Interval: {args.interval}s")
    print()

    fleet = FleetSimulator(args.host, args.port, args.fleet, args.count)

    # Handle Ctrl+C gracefully
    def signal_handler(sig, frame):
        fleet.stop()
        sys.exit(0)

    signal.signal(signal.SIGINT, signal_handler)

    fleet.start(args.interval)

    # Keep main thread alive
    try:
        while True:
            time.sleep(1)
    except KeyboardInterrupt:
        fleet.stop()


if __name__ == "__main__":
    main()
