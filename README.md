# AI-Powered Legal Strategy Advisor – Project Overview

## Introduction
The Legal Strategy Advisor project is designed to provide a comprehensive solution for legal strategy formulation using AI technologies. This project is structured into multiple modular components, each serving a specific purpose, ensuring maintainability and scalability.

A comprehensive legal strategy recommendation system built with .NET 8, Python FastAPI, and Angular, featuring AI-powered analysis with secure fallbacks.

## 🚀 Quick Start

### Option 1: Docker (Recommended)

1. **Clone and setup**:
   ```bash
   git clone https://github.com/iirimia/LegalStrategyAdvisor.git
   cd LegalStrategyAdvisor
   cp .env.example .env
   ```

2. **Configure environment** (edit `.env`):
   ```env
   POSTGRES_CONNECTION=Host=localhost;Port=5432;Database=legal_strategy;Username=postgres;Password=postgres
   AI_SERVICE_URL=http://localhost:8001
   MODEL_PROVIDER=fake  # or "openai" for real AI
   OPENAI_API_KEY=      # Required when MODEL_PROVIDER=openai
   ```

3. **Start services**:
   ```bash
   podman-compose up --build
   # or: docker-compose up --build
   ```

4. **Access applications**:
   - **API Documentation**: http://localhost:5000/swagger
   - **AI Service Health**: http://localhost:8001/health
   - **Frontend** (when built): http://localhost:4200

### Option 2: Local Development

**Prerequisites**:
- .NET 8 SDK
- Python 3.11+
- Node.js 20+
- PostgreSQL 15+

**Setup**:

1. **Database**:
   ```bash
   # Apply migrations
   dotnet ef database update --project Api
   ```

2. **Backend API**:
   ```bash
   cd Api
   dotnet run
   # Runs on http://localhost:5000
   ```

3. **AI Service**:
   ```bash
   cd AiService
   pip install -r requirements.txt
   uvicorn main:app --host 0.0.0.0 --port 8001
   ```

4. **Frontend**:
   ```bash
   cd Frontend
   npm install
   npm start
   # Runs on http://localhost:4200
   ```

## 🏗️ Architecture

### Tech Stack
- **Backend API**: .NET 8 Minimal APIs, EF Core (PostgreSQL), FluentValidation patterns
- **AI Service**: Python FastAPI, Pydantic v2, OpenAI integration
- **Frontend**: Angular 18 standalone components, Reactive Forms
- **Database**: PostgreSQL 15+ with EF Core migrations
- **Containerization**: Docker/Podman support

### Project Structure
```
LegalStrategyAdvisor/
├─ Api/                     # .NET 8 Minimal API
│  ├─ Controllers/          # API controllers
│  ├─ Data/                 # EF Core context & entities
│  ├─ Services/             # Business logic & AI integration
│  └─ DTOs/                 # Data transfer objects
├─ AiService/               # Python FastAPI microservice
│  ├─ app/                  # FastAPI application
│  ├─ providers/            # AI provider implementations
│  └─ main.py               # FastAPI entry point
├─ Frontend/                # Angular application
│  ├─ src/app/              # Angular components & services
│  └─ environments/         # Environment configurations
├─ Database/                # Database scripts
├─ .vscode/                 # VS Code configurations
└─ .github/                 # GitHub workflows & Copilot instructions
```

## 🔧 Features

### AI-Powered Legal Analysis
- **Multiple AI Providers**: OpenAI, Azure OpenAI, with intelligent fallbacks
- **Secure by Default**: Input validation, PII redaction, content filtering
- **Deterministic Fallback**: Mock provider ensures system availability
- **Configurable Models**: Environment-based provider selection

### Legal Precedent Explorer
- **Search & Filter**: By jurisdiction, case type, year, relevance score
- **Rich Metadata**: Full case citations, summaries, relevance scoring
- **Performance Optimized**: Database indexes for fast queries
- **Seed Data**: Pre-loaded with landmark legal precedents

### Security & Compliance
- **Input Validation**: Length limits, character validation, enum constraints
- **PII Protection**: Basic redaction of sensitive information
- **Rate Limiting**: Configurable limits on public routes
- **CORS Protection**: Allowlist-based cross-origin policies
- **Secure Logging**: Context-aware logging without secrets/PII

## 🔐 Configuration

### Environment Variables

**.NET API** (appsettings.json):
```json
{
  "AiProvider": {
    "Provider": "PythonAI",
    "EnableFallback": true,
    "MaxRetries": 2,
    "TimeoutSeconds": 15,
    "OpenAI": {
      "ApiKey": "",
      "Model": "gpt-4o-mini"
    },
    "PythonAI": {
      "BaseUrl": "http://localhost:8001"
    }
  }
}
```

**Python AI Service** (.env):
```env
MODEL_PROVIDER=fake          # "fake" | "openai"
OPENAI_API_KEY=             # Required for OpenAI
OPENAI_MODEL=gpt-4o-mini    # Model selection
OPENAI_MAX_TOKENS=512       # Response length
OPENAI_TEMPERATURE=0.7      # Creativity level
```

### AI Provider Selection

The system supports multiple AI providers with automatic fallback:

1. **PythonAI** → Routes to Python FastAPI service
2. **OpenAI** → Direct OpenAI integration (.NET)
3. **AzureOpenAI** → Azure OpenAI integration
4. **Mock** → Deterministic fallback (always available)

## 📊 API Endpoints

### Core Strategy API
```http
POST /api/strategy/generate
Content-Type: application/json

{
  "caseDescription": "Contract dispute involving delayed delivery"
}
```

### Precedent Explorer
```http
GET /api/precedents/search?query=contract&jurisdiction=Supreme%20Court&limit=5
GET /api/precedents/{id}
GET /api/precedents/case-types
GET /api/precedents/jurisdictions
```

### Health & Status
```http
GET /api/health              # System health
GET /api/strategy/status     # AI service status
```

## 🧪 Testing

### Unit Tests
```bash
# .NET API tests
cd Api.Tests
dotnet test

# Python service tests
cd AiService
pytest

# Frontend tests
cd Frontend
npm test
```

### Integration Testing
```bash
# Test AI providers
curl -X POST http://localhost:8001/api/strategy \
  -H "Content-Type: application/json" \
  -d '{"case_description": "Test case"}'

# Test .NET to Python integration
curl -X POST http://localhost:5000/api/strategy/generate \
  -H "Content-Type: application/json" \
  -d '{"caseDescription": "Test case"}'
```

## 🚀 Deployment

### Production Considerations

1. **Environment Variables**:
   - Set real OpenAI API keys
   - Configure production database connections
   - Enable HTTPS and HSTS
   - Set appropriate CORS origins

2. **Security Hardening**:
   - Enable rate limiting
   - Configure proper logging levels
   - Set up monitoring and alerting
   - Regular security updates

3. **Performance**:
   - Database connection pooling
   - Redis caching (optional)
   - Load balancing for multiple instances

## 🤝 Contributing

This project follows secure coding practices defined in [.github/copilot-instructions.md](.github/copilot-instructions.md):

- **Security First**: Input validation, parameterized queries, no hardcoded secrets
- **Clean Architecture**: Small functions, strong typing, explicit error handling
- **Testable Design**: DI patterns, avoid singletons, separation of concerns

### Development Workflow

1. Create feature branch
2. Follow coding guidelines in `.github/copilot-instructions.md`
3. Add unit tests for new features
4. Test with both mock and real AI providers
5. Create pull request with comprehensive description

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🔗 Links

- **GitHub Repository**: https://github.com/iirimia/LegalStrategyAdvisor
- **Issues & Feedback**: https://github.com/iirimia/LegalStrategyAdvisor/issues
- **OpenAI Documentation**: https://platform.openai.com/docs

---

**Built with ❤️ using .NET 8, Python FastAPI, and Angular**
## Project Structure
The project is organized into the following main directories:

- **Api**: Contains the .NET 8 Minimal API for handling legal strategy requests.
  - **Controllers**: Manages API request handling.
  - **Data**: Contains data-related files, including entity definitions and the database context.
  - **DTOs**: Data Transfer Objects for API communication.
  - **Services**: Business logic services.
  - **Middleware**: Custom middleware for request processing.
  - **Migrations**: Database migration files.
  - **Program.cs**: Entry point for the .NET application.
  - **appsettings.json**: Configuration settings for the application.
  - **appsettings.Development.json**: Development-specific configuration settings.
  - **LegalStrategyAdvisor.Api.csproj**: Project file for the .NET API.
  - **Dockerfile**: Instructions for building the Docker image for the API.

- **AiService**: Contains the Python FastAPI service for AI-driven strategy suggestions.
  - **app**: Main application code for the FastAPI service.
    - **models**: Model definitions for request and response data.
    - **routers**: Route definitions for the FastAPI application.
    - **services**: Business logic services.
  - **main.py**: Entry point for the FastAPI application.
  - **requirements.txt**: Lists dependencies for the Python service.
  - **Dockerfile**: Instructions for building the Docker image for the AI service.

- **Frontend**: Contains the Angular application for user interaction.
  - **src**: Source code for the Angular application.
    - **app**: Main application components and services.
    - **components**: Reusable components.
    - **services**: API communication services.
    - **models**: Model definitions for data structures.
    - **app.component.ts**: Root component of the Angular application.
    - **environments**: Environment configuration files.
    - **main.ts**: Entry point for the Angular application.
  - **angular.json**: Configuration settings for the Angular project.
  - **package.json**: Lists dependencies and scripts for the Angular application.
  - **tsconfig.json**: TypeScript compiler options.

- **Aspire**: Contains the host application and service defaults for Aspire.
  - **AppHost**: Host application for Aspire.
  - **ServiceDefaults**: Default service configurations.

- **Database**: Contains scripts and migration files for database management.

- **.vscode**: Contains configuration files for the development environment in VS Code.

- **.github**: Contains workflows for CI/CD and Copilot instructions.

- **devcontainer**: Configuration settings for the development container.

## Getting Started
To get started with the Legal Strategy Advisor project, follow these steps:

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd LegalStrategyAdvisor
   ```

2. **Set up environment variables**:
   Copy `.env.example` to `.env` and fill in the required values.

3. **Run the application**:
   Use Docker to build and run the application:
   ```bash
   docker-compose up --build
   ```

4. **Access the services**:
   - API Swagger: [http://localhost:5199/swagger](http://localhost:5199/swagger)
   - AI Service Docs: [http://localhost:8001/docs](http://localhost:8001/docs)
   - Angular (dev server): [http://localhost:4200](http://localhost:4200)

## Contributing
Contributions are welcome! Please follow the standard Git workflow for submitting changes.

## License
This project is licensed under the MIT License. See the LICENSE file for details.