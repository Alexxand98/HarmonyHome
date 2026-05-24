export interface ResponseApi<T> {
  statusCode: number;
  isSuccess: boolean;
  result: T | null;
  errorMessages: string[];
}