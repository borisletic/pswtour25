export enum Interest {
  Nature = 1,
  Art = 2,
  Sport = 3,
  Shopping = 4,
  Food = 5
}

export enum TourDifficulty {
  Easy = 1,
  Medium = 2,
  Hard = 3,
  Expert = 4
}

export enum TourStatus {
  Draft = 1,
  Published = 2,
  Cancelled = 3
}

export interface Location {
  latitude: number;
  longitude: number;
}

export interface KeyPoint {
  id: string;
  name: string;
  description: string;
  latitude: number;
  longitude: number;
  imageUrl?: string;
  order: number;
}

export interface GuideInfo {
  id: string;
  firstName: string;
  lastName: string;
  isRewarded: boolean;
}

export interface Tour {
  id: string;
  name: string;
  description: string;
  difficulty: string;
  category: string;
  price: number;
  currency: string;
  scheduledDate: Date;
  status: string;
  averageRating: number;
  ratingsCount: number;
  keyPoints: KeyPoint[];
  guide: GuideInfo;
}

export interface TourFilter {
  category?: Interest;
  difficulty?: TourDifficulty;
  status?: string;  // Changed to string since backend expects string
  guideId?: string;
  rewardedGuidesOnly?: boolean;
}

export interface CreateTourRequest {
  name: string;
  description: string;
  difficulty: TourDifficulty;
  category: Interest;
  price: number;
  scheduledDate: Date;
}