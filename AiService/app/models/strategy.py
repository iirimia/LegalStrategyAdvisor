from pydantic import BaseModel, Field, field_validator
from typing import Optional
import re

class StrategyRequest(BaseModel):
    """Request model for legal strategy generation"""
    case_description: str = Field(
        ..., 
        min_length=10, 
        max_length=5000,
        description="Description of the legal case or matter"
    )
    
    @field_validator('case_description')
    @classmethod
    def validate_case_description(cls, v):
        # Basic input sanitization
        if not v or not v.strip():
            raise ValueError('Case description cannot be empty')
        
        # Remove potentially dangerous characters
        sanitized = re.sub(r'[<>"\']', '', v.strip())
        
        # Basic PII redaction (simple patterns)
        # SSN pattern
        sanitized = re.sub(r'\b\d{3}-?\d{2}-?\d{4}\b', '[SSN REDACTED]', sanitized)
        # Phone pattern
        sanitized = re.sub(r'\b\d{3}[-.]?\d{3}[-.]?\d{4}\b', '[PHONE REDACTED]', sanitized)
        # Email pattern
        sanitized = re.sub(r'\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,}\b', '[EMAIL REDACTED]', sanitized)
        
        return sanitized

class StrategyResponse(BaseModel):
    """Response model for legal strategy generation"""
    strategy: str = Field(..., description="Generated legal strategy")
    provider: str = Field(..., description="AI provider used")
    tokens_used: Optional[int] = Field(None, description="Number of tokens used")
    processing_time: Optional[float] = Field(None, description="Processing time in seconds")

class HealthResponse(BaseModel):
    """Health check response model"""
    status: str = Field(..., description="Service status")
    provider: str = Field(..., description="Current AI provider")
    provider_available: bool = Field(..., description="Whether the AI provider is available")
