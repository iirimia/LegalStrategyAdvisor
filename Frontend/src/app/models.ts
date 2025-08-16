export interface StrategyRequest {
  readonly caseType: CaseType;
  readonly jurisdiction: string;
  readonly description: string;
  readonly clientGoals: readonly string[];
}

export interface StrategyResponse {
  readonly id: string;
  readonly strategies: readonly string[];
  readonly precedents: readonly Precedent[];
  readonly riskAssessment: string;
  readonly estimatedDuration: string;
  readonly confidence: number;
}

export interface Precedent {
  readonly id: number;
  readonly citation: string;
  readonly summary: string;
  readonly jurisdiction: string;
  readonly year: number;
  readonly caseType: string;
  readonly relevanceScore: number;
}

export interface PrecedentSearchRequest {
  readonly jurisdiction?: string;
  readonly caseType?: string;
  readonly year?: number;
  readonly searchTerm?: string;
  readonly pageSize?: number;
  readonly page?: number;
}

export interface PrecedentSearchResponse {
  readonly precedents: readonly Precedent[];
  readonly totalCount: number;
  readonly page: number;
  readonly pageSize: number;
  readonly totalPages: number;
}

export interface ApiError {
  readonly detail: string;
  readonly type?: string;
  readonly title?: string;
  readonly status?: number;
}

export type CaseType =
  | "civil"
  | "criminal"
  | "corporate"
  | "family"
  | "intellectual-property"
  | "employment";

export const CASE_TYPES: readonly CaseType[] = [
  "civil",
  "criminal",
  "corporate",
  "family",
  "intellectual-property",
  "employment",
] as const;
