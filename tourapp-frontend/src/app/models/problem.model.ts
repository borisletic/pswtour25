export enum ProblemStatus {
  Pending = 'Pending',
  Resolved = 'Resolved',
  UnderReview = 'UnderReview',
  Rejected = 'Rejected'
}

export interface Problem {
  id: string;
  tourId: string;
  tourName: string;
  touristId: string;
  touristName: string;
  title: string;
  description: string;
  status: ProblemStatus;
  createdAt: Date;
  events: ProblemEvent[];
}

export interface ProblemEvent {
  occurredAt: Date;
  type: string;
  oldStatus?: ProblemStatus;
  newStatus?: ProblemStatus;
}