// Wire shapes mirroring backend records. Keep in sync with dotRAG.API/Models
// and dotRAG.API/Application/PipelineTrace.cs. Web JSON casing (camelCase).

export interface ChunkDto {
  sourceFile: string;
  heading: string;
  content: string;
  score: number | null;
}

export type Role = 'user' | 'assistant';

export interface HistoryMessage {
  role: Role;
  content: string;
}

export interface ChatRequest {
  question: string;
  history?: HistoryMessage[];
}

export interface ChatResponse {
  answer: string;
  chunks: ChunkDto[];
}

export interface StageTiming {
  ms: number;
  meta: Record<string, unknown>;
}

export type StageKind =
  | 'queryRewrite'
  | 'embedding'
  | 'vectorSearch'
  | 'promptBuild'
  | 'llmComplete';

// Polled trace shape. All stages nullable — in-flight traces only have the
// stages that have finished so far. Backend flips `running` to false on completion.
export interface PipelineTrace {
  correlationId: string;
  timestamp: string;
  question: string;
  rewrittenQuery: string | null;
  queryRewrite: StageTiming | null;
  embedding: StageTiming | null;
  vectorSearch: StageTiming | null;
  promptBuild: StageTiming | null;
  llmComplete: StageTiming | null;
  totalMs: number;
  running: boolean;
  retrievedChunks: ChunkDto[];
}

export interface RecentTraceSummary {
  correlationId: string;
  timestamp: string;
  question: string;
  totalMs: number;
  running: boolean;
}
