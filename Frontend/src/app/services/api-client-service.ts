// filepath: c:\dev\IonelaFolder\_Projects-Ionela_\Learning\LegalStrategyAdvisor\Frontend\src\app\services\api-client.service.ts
import { Injectable, inject } from "@angular/core";
import {
  HttpClient,
  HttpErrorResponse,
  HttpHeaders,
} from "@angular/common/http";
import { Observable, throwError, timeout, retry } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { environment } from "../../environments/environment";
import { 
  StrategyRequest, 
  StrategyResponse, 
  ApiError, 
  Precedent, 
  PrecedentSearchRequest, 
  PrecedentSearchResponse 
} from "../models";

@Injectable({
  providedIn: "root",
})
export class ApiClientService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = environment.apiUrl;
  private readonly timeoutMs = 30000;
  private readonly maxRetries = 2;

  private readonly defaultHeaders = new HttpHeaders({
    "Content-Type": "application/json",
    Accept: "application/json",
  });

  suggestStrategy(request: StrategyRequest): Observable<StrategyResponse> {
    // Input validation following security defaults
    this.validateStrategyRequest(request);

    return this.http
      .post<StrategyResponse>(
        `${this.baseUrl}/api/strategies/suggest`,
        request,
        { headers: this.defaultHeaders }
      )
      .pipe(
        timeout(this.timeoutMs),
        retry(this.maxRetries),
        map((response) => this.validateResponse(response)),
        catchError(this.handleError.bind(this))
      );
  }

  // Precedent Explorer Methods
  searchPrecedents(request: PrecedentSearchRequest = {}): Observable<PrecedentSearchResponse> {
    this.validatePrecedentSearchRequest(request);
    
    const params = this.buildSearchParams(request);
    
    return this.http
      .get<PrecedentSearchResponse>(`${this.baseUrl}/api/precedents`, { 
        headers: this.defaultHeaders,
        params 
      })
      .pipe(
        timeout(this.timeoutMs),
        retry(this.maxRetries),
        catchError(this.handleError.bind(this))
      );
  }

  getPrecedent(id: number): Observable<Precedent> {
    if (id <= 0) {
      throw new Error('Invalid precedent ID');
    }

    return this.http
      .get<Precedent>(`${this.baseUrl}/api/precedents/${id}`, { 
        headers: this.defaultHeaders 
      })
      .pipe(
        timeout(this.timeoutMs),
        retry(this.maxRetries),
        catchError(this.handleError.bind(this))
      );
  }

  getCaseTypes(): Observable<string[]> {
    return this.http
      .get<string[]>(`${this.baseUrl}/api/precedents/case-types`, { 
        headers: this.defaultHeaders 
      })
      .pipe(
        timeout(this.timeoutMs),
        retry(this.maxRetries),
        catchError(this.handleError.bind(this))
      );
  }

  getJurisdictions(): Observable<string[]> {
    return this.http
      .get<string[]>(`${this.baseUrl}/api/precedents/jurisdictions`, { 
        headers: this.defaultHeaders 
      })
      .pipe(
        timeout(this.timeoutMs),
        retry(this.maxRetries),
        catchError(this.handleError.bind(this))
      );
  }

  private buildSearchParams(request: PrecedentSearchRequest): Record<string, string> {
    const params: Record<string, string> = {};
    
    if (request.jurisdiction) params['jurisdiction'] = request.jurisdiction;
    if (request.caseType) params['caseType'] = request.caseType;
    if (request.year) params['year'] = request.year.toString();
    if (request.searchTerm) params['searchTerm'] = request.searchTerm;
    if (request.pageSize) params['pageSize'] = request.pageSize.toString();
    if (request.page) params['page'] = request.page.toString();
    
    return params;
  }

  private validatePrecedentSearchRequest(request: PrecedentSearchRequest): void {
    if (request.jurisdiction && request.jurisdiction.length > 100) {
      throw new Error('Jurisdiction must be under 100 characters');
    }
    if (request.caseType && request.caseType.length > 50) {
      throw new Error('Case type must be under 50 characters');
    }
    if (request.searchTerm && request.searchTerm.length > 200) {
      throw new Error('Search term must be under 200 characters');
    }
    if (request.year && (request.year < 1000 || request.year > 2100)) {
      throw new Error('Year must be between 1000 and 2100');
    }
    if (request.pageSize && (request.pageSize < 1 || request.pageSize > 100)) {
      throw new Error('Page size must be between 1 and 100');
    }
    if (request.page && request.page < 1) {
      throw new Error('Page must be 1 or greater');
    }
  }
        catchError(this.handleError.bind(this))
      );
  }

  private validateStrategyRequest(request: StrategyRequest): void {
    if (!request.caseType || request.caseType.length > 50) {
      throw new Error("Invalid case type");
    }
    if (!request.jurisdiction || request.jurisdiction.length > 100) {
      throw new Error("Invalid jurisdiction");
    }
    if (!request.description || request.description.length > 2000) {
      throw new Error("Description must be between 1-2000 characters");
    }
    if (request.clientGoals.some((goal) => goal.length > 200)) {
      throw new Error("Individual client goals must be under 200 characters");
    }
  }

  private validateResponse(response: any): StrategyResponse {
    // Validate external response following security defaults
    if (!response || typeof response !== "object") {
      throw new Error("Invalid response format");
    }

    return {
      id: this.sanitizeString(response.id),
      strategies: Array.isArray(response.strategies)
        ? response.strategies.map((s: any) => this.sanitizeString(s))
        : [],
      precedents: Array.isArray(response.precedents) ? response.precedents : [],
      riskAssessment: this.sanitizeString(response.riskAssessment),
      estimatedDuration: this.sanitizeString(response.estimatedDuration),
      confidence: Math.max(0, Math.min(1, Number(response.confidence) || 0)),
    };
  }

  private sanitizeString(value: any): string {
    if (typeof value !== "string") return "";
    // Basic HTML stripping and length control
    return value.replace(/<[^>]*>/g, "").substring(0, 1000);
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = "An unexpected error occurred";

    if (error.status === 0) {
      errorMessage = "Network error - please check your connection";
    } else if (error.status >= 400 && error.status < 500) {
      const apiError = error.error as ApiError;
      errorMessage = apiError?.detail || "Client error occurred";
    } else if (error.status >= 500) {
      errorMessage = "Server error - please try again later";
    }

    // Log with context but never secrets/PII
    console.error(`API Error [${error.status}]:`, {
      url: error.url,
      message: errorMessage,
      timestamp: new Date().toISOString(),
    });

    return throwError(() => new Error(errorMessage));
  }
}
