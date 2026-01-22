#!/bin/bash

# Start development servers for Route Planner
echo "Starting Route Planner development environment..."

# Function to cleanup background processes on script exit
cleanup() {
    echo "Stopping development servers..."
    kill $FRONTEND_PID $BACKEND_PID 2>/dev/null
    exit 0
}

# Set up trap to cleanup on script termination
trap cleanup EXIT INT TERM

# Start Angular frontend
echo "Starting Angular frontend..."
cd route-planner-frontend
npm start &
FRONTEND_PID=$!
cd ..

# Start .NET backend
echo "Starting .NET backend..."
cd RoutePlanner
dotnet run &
BACKEND_PID=$!
cd ..

echo "Both servers started:"
echo "  Frontend PID: $FRONTEND_PID"
echo "  Backend PID: $BACKEND_PID"
echo "Press Ctrl+C to stop both servers"

# Wait for both processes
wait $FRONTEND_PID $BACKEND_PID