import os
import logging
from fastapi import FastAPI
from fastapi.middleware.cors import CORSMiddleware
from dotenv import load_dotenv
from app.routers import strategy_router

# Load environment variables
load_dotenv()

# Configure logging
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s'
)
logger = logging.getLogger(__name__)

# Initialize FastAPI app
app = FastAPI(
    title="Legal Strategy AI Service",
    description="AI-powered legal strategy generation service",
    version="1.0.0"
)

# CORS configuration - allow frontend access
origins = [
    "http://localhost:4200",  # Angular frontend
    "http://localhost:3000",  # Alternative frontend port
    "http://localhost:5000",  # .NET API
]

app.add_middleware(
    CORSMiddleware,
    allow_origins=origins,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Include routers
app.include_router(strategy_router, prefix="/api")

# Root health endpoint
@app.get("/health")
def root_health_check():
    """Root health check endpoint"""
    model_provider = os.getenv("MODEL_PROVIDER", "fake")
    return {
        "status": "healthy",
        "service": "Legal Strategy AI Service",
        "provider": model_provider
    }

@app.get("/")
def read_root():
    """Root endpoint"""
    return {
        "message": "Legal Strategy AI Service",
        "version": "1.0.0",
        "docs": "/docs"
    }

if __name__ == "__main__":
    import uvicorn
    
    # Get configuration from environment
    host = os.getenv("HOST", "0.0.0.0")
    port = int(os.getenv("PORT", "8001"))
    
    logger.info(f"Starting Legal Strategy AI Service on {host}:{port}")
    logger.info(f"AI Provider: {os.getenv('MODEL_PROVIDER', 'fake')}")
    
    uvicorn.run(
        app, 
        host=host, 
        port=port,
        log_level="info"
    )