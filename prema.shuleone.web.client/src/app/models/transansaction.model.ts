export interface TransactionResult {
    transactionId: string; // Maps to TransactionId in C#
    status: number; // Maps to TransactionStatus in C#
    message: string; // Maps to Message in C#
}

export enum TransactionStatus {
    Success = "Success", // Enum values are string representations
    Pending = "Pending",
    Failed = "Failed",
    NotFound = "NotFound"
}
