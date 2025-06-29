// image-compression.service.ts
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class FileCompressionService {

  async compressFile(file: File, options?: CompressionOptions): Promise<File> {
    const settings = {
      maxSizeMB: 2, // Target size
      maxWidthOrHeight: 1920, // Max dimension
      quality: 0.8, // 80% quality
      fileType: 'image/jpeg', // Convert everything to JPEG
      ...options
    };

    // If it's not an image, return as-is
    if (!file.type.startsWith('image/')) {
      return file;
    }

    // If already small enough, return as-is
    if (file.size <= settings.maxSizeMB * 1024 * 1024) {
      console.log(`File already small enough: ${(file.size / 1024 / 1024).toFixed(2)}MB`);
      return file;
    }

    console.log(`Compressing: ${file.name} (${(file.size / 1024 / 1024).toFixed(2)}MB)`);

    return new Promise((resolve, reject) => {
      const img = new Image();
      const canvas = document.createElement('canvas');
      const ctx = canvas.getContext('2d');

      if (!ctx) {
        reject(new Error('Could not get canvas context'));
        return;
      }

      img.onload = () => {
        // Calculate new dimensions
        const { width, height } = this.calculateDimensions(
          img.width,
          img.height,
          settings.maxWidthOrHeight
        );

        canvas.width = width;
        canvas.height = height;

        // Draw and compress
        ctx.drawImage(img, 0, 0, width, height);

        canvas.toBlob(
          (blob) => {
            if (!blob) {
              reject(new Error('Compression failed'));
              return;
            }

            const compressedFile = new File(
              [blob],
              this.getCompressedFileName(file.name),
              {
                type: settings.fileType,
                lastModified: Date.now()
              }
            );

            console.log(`Compressed: ${(file.size / 1024 / 1024).toFixed(2)}MB → ${(compressedFile.size / 1024 / 1024).toFixed(2)}MB`);
            resolve(compressedFile);
          },
          settings.fileType,
          settings.quality
        );

        // Clean up
        URL.revokeObjectURL(img.src);
      };

      img.onerror = () => {
        reject(new Error('Failed to load image'));
      };

      img.src = URL.createObjectURL(file);
    });
  }

  private calculateDimensions(originalWidth: number, originalHeight: number, maxSize: number) {
    if (originalWidth <= maxSize && originalHeight <= maxSize) {
      return { width: originalWidth, height: originalHeight };
    }

    const ratio = Math.min(maxSize / originalWidth, maxSize / originalHeight);
    return {
      width: Math.round(originalWidth * ratio),
      height: Math.round(originalHeight * ratio)
    };
  }

  private getCompressedFileName(originalName: string): string {
    const nameWithoutExt = originalName.replace(/\.[^/.]+$/, '');
    return `${nameWithoutExt}_compressed.jpg`;
  }

  // Multiple compression attempts with different quality levels
  async compressToTarget(file: File, targetSizeMB: number = 2): Promise<File> {
    if (!file.type.startsWith('image/')) return file;
    if (file.size <= targetSizeMB * 1024 * 1024) return file;

    const qualities = [0.9, 0.8, 0.7, 0.6, 0.5, 0.4];

    for (const quality of qualities) {
      const compressed = await this.compressFile(file, { quality });

      if (compressed.size <= targetSizeMB * 1024 * 1024) {
        return compressed;
      }
    }

    // If still too large, compress dimensions more aggressively
    return this.compressFile(file, {
      quality: 0.4,
      maxWidthOrHeight: 1280
    });
  }
}

export interface CompressionOptions {
  maxSizeMB?: number;
  maxWidthOrHeight?: number;
  quality?: number;
  fileType?: string;
}
