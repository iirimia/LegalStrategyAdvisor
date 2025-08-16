import os
import openai
from pydantic import BaseModel
from typing import Optional
import logging

logger = logging.getLogger(__name__)

class OpenAIConfig(BaseModel):
    api_key: str
    model: str = "gpt-4o-mini"
    max_tokens: int = 512
    temperature: float = 0.7

class OpenAIProvider:
    def __init__(self):
        self.config = OpenAIConfig(
            api_key=os.environ.get("OPENAI_API_KEY", ""),
            model=os.environ.get("OPENAI_MODEL", "gpt-4o-mini"),
            max_tokens=int(os.environ.get("OPENAI_MAX_TOKENS", "512")),
            temperature=float(os.environ.get("OPENAI_TEMPERATURE", "0.7"))
        )
        
        if self.config.api_key:
            openai.api_key = self.config.api_key
            logger.info(f"OpenAI provider initialized with model: {self.config.model}")
        else:
            logger.warning("OpenAI API key not provided")

    async def generate(self, prompt: str) -> str:
        """Generate AI response using OpenAI API"""
        if not self.config.api_key:
            return "[Error: OpenAI API key missing]"
        
        try:
            # Use the newer client pattern
            client = openai.AsyncOpenAI(api_key=self.config.api_key)
            
            system_prompt = """You are a legal strategy advisor AI assistant. 
            Provide comprehensive, well-structured legal analysis and strategy recommendations. 
            Always include relevant legal precedents, potential risks, and actionable steps. 
            Format your response clearly with sections and bullet points where appropriate.
            Keep responses professional and within the specified token limits."""
            
            response = await client.chat.completions.create(
                model=self.config.model,
                messages=[
                    {"role": "system", "content": system_prompt},
                    {"role": "user", "content": prompt}
                ],
                max_tokens=self.config.max_tokens,
                temperature=self.config.temperature,
                timeout=30.0  # 30 second timeout
            )
            
            content = response.choices[0].message.content
            if content:
                logger.info(f"OpenAI response generated successfully. Tokens used: {response.usage.total_tokens if response.usage else 'unknown'}")
                return content.strip()
            else:
                return "[Error: Empty response from OpenAI]"
                
        except openai.RateLimitError as e:
            logger.error(f"OpenAI rate limit error: {e}")
            return "[Error: Rate limit exceeded. Please try again later.]"
        except openai.AuthenticationError as e:
            logger.error(f"OpenAI authentication error: {e}")
            return "[Error: Invalid API key]"
        except openai.APITimeoutError as e:
            logger.error(f"OpenAI timeout error: {e}")
            return "[Error: Request timeout]"
        except Exception as e:
            logger.error(f"OpenAI error: {e}")
            return f"[OpenAI error: {str(e)}]"

    def is_available(self) -> bool:
        """Check if OpenAI provider is properly configured"""
        return bool(self.config.api_key)
