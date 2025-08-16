from fastapi import APIRouter, HTTPException, Depends
from app.models.strategy import StrategyRequest, StrategyResponse, HealthResponse
import os
import time
import logging

logger = logging.getLogger(__name__)

# Initialize AI provider based on environment variable
MODEL_PROVIDER = os.getenv("MODEL_PROVIDER", "fake").lower()

if MODEL_PROVIDER == "openai":
    from providers.openai_provider import OpenAIProvider
    ai_provider = OpenAIProvider()
else:
    from providers.fake_provider import FakeProvider
    ai_provider = FakeProvider()

router = APIRouter()

@router.post("/strategy", response_model=StrategyResponse)
async def generate_strategy(request: StrategyRequest):
    """Generate legal strategy based on case description"""
    try:
        start_time = time.time()
        
        # Create prompt for AI provider
        prompt = f"Suggest a comprehensive legal strategy for the following case: {request.case_description}"
        
        # Generate strategy using the configured AI provider
        strategy = await ai_provider.generate(prompt)
        
        processing_time = time.time() - start_time
        
        # Log the request (without sensitive data)
        logger.info(f"Strategy generated using {MODEL_PROVIDER} provider in {processing_time:.2f}s")
        
        return StrategyResponse(
            strategy=strategy,
            provider=MODEL_PROVIDER,
            tokens_used=None,  # Could be enhanced to track actual tokens
            processing_time=processing_time
        )
        
    except Exception as e:
        logger.error(f"Error generating strategy: {e}")
        raise HTTPException(
            status_code=500, 
            detail="An error occurred while generating the legal strategy"
        )

@router.get("/health", response_model=HealthResponse)
async def health_check():
    """Health check endpoint with AI provider status"""
    try:
        provider_available = ai_provider.is_available()
        
        return HealthResponse(
            status="healthy",
            provider=MODEL_PROVIDER,
            provider_available=provider_available
        )
        
    except Exception as e:
        logger.error(f"Health check error: {e}")
        return HealthResponse(
            status="degraded",
            provider=MODEL_PROVIDER,
            provider_available=False
        )

# Export the router
strategy_router = router
