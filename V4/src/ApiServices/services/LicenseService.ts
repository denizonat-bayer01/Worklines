import { API_ROUTES } from '../config/api.config';
import { TokenService } from './TokenService';

export interface LicenseValidationRequest {
  licenseKey: string;
  machineCode?: string;
  clientVersion?: string;
}

export interface LicenseStatus {
  isValid: boolean;
  isExpired: boolean;
  expireDate?: string;
  daysRemaining: number;
  tenantCompanyName?: string;
  lastValidatedAt?: string;
  licenseKey?: string;
}

export interface LicenseValidationResponse {
  success: boolean;
  message?: string;
  data?: {
    isValid: boolean;
    expireDate?: string;
    daysRemaining: number;
    tenantCompanyName?: string;
  };
}

class LicenseService {
  private tokenService = TokenService.getInstance();

  private async getAuthHeader() {
    const token = await this.tokenService.getToken();
    return {
      'Authorization': `Bearer ${token}`,
      'Content-Type': 'application/json',
    };
  }

  // Admin: Validate and save license key
  async validateLicense(licenseKey: string, machineCode?: string, clientVersion?: string): Promise<LicenseValidationResponse> {
    try {
      // Don't require auth header for license validation (AllowAnonymous endpoint)
      const response = await fetch(
        `${API_ROUTES.BASE_URL}/api/v1.0/admin/license/validate`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify({
            licenseKey,
            machineCode,
            clientVersion,
          }),
        }
      );

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || 'Lisans doğrulanamadı');
      }

      return await response.json();
    } catch (error) {
      console.error('Error validating license:', error);
      throw error;
    }
  }

  // Admin: Get license status
  async getLicenseStatus(): Promise<LicenseStatus> {
    try {
      // Don't require auth header for license status (AllowAnonymous endpoint)
      const response = await fetch(
        `${API_ROUTES.BASE_URL}/api/v1.0/admin/license/status`,
        {
          headers: {
            'Content-Type': 'application/json',
          },
        }
      );

      if (!response.ok) {
        throw new Error('Lisans durumu alınamadı');
      }

      const result = await response.json();
      return result.data;
    } catch (error) {
      console.error('Error getting license status:', error);
      throw error;
    }
  }

  // Public: Get license status (only valid/invalid)
  async getPublicLicenseStatus(): Promise<{ isValid: boolean; isExpired: boolean }> {
    try {
      // Create AbortController for timeout
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), 5000); // 5 second timeout

      const response = await fetch(
        `${API_ROUTES.BASE_URL}/api/v1.0/license/status`,
        {
          signal: controller.signal
        }
      );

      clearTimeout(timeoutId);

      if (!response.ok) {
        // If API fails (500, network error, etc.), throw error to be handled by caller
        throw new Error(`License API returned ${response.status}`);
      }

      const result = await response.json();
      return result.data || { isValid: false, isExpired: true };
    } catch (error) {
      console.error('Error getting public license status:', error);
      // On error, throw to be handled by caller (LicenseRouteGuard will allow access)
      throw error;
    }
  }
}

const licenseService = new LicenseService();
export default licenseService;

