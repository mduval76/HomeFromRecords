export interface AlbumDto {
  albumId: string;
  title: string;
  price?: number;
  quantity: number;
  country: string;
  releaseYear: string;
  catalogNumber: string;
  matrixNumber: string;
  imgFileExt: string;
  details: string;
  artistName: string;
  recordLabelName: string;
  mediaGrade: number;
  sleeveGrade: number;
  format: number;
  subFormat: number;
  packageType: number;
  vinylSpeed: number;
  artistGenre: number;
  albumGenre: number;
  albumLength: number;
  albumType: number;

  mediaGradeName?: string;
  sleeveGradeName?: string;
  formatName?: string;
  subFormatName?: string;
  packageTypeName?: string;
  vinylSpeedName?: string;
  artistGenreName?: string;
  albumGenreName?: string;
  albumLengthName?: string;
  albumTypeName?: string;
}
