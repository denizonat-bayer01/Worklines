import { useEffect, useState, type ReactNode } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import licenseService from '@/ApiServices/services/LicenseService';
import Maintenance from '@/pages/Maintenance';
import LicenseKeyEntry from '@/pages/Admin/LicenseKeyEntry';
import { Loader2 } from 'lucide-react';

interface LicenseRouteGuardProps {
  children: ReactNode;
  isAdminRoute?: boolean;
  isPublicRoute?: boolean;
}

const LicenseRouteGuard = ({ children, isAdminRoute = false, isPublicRoute = false }: LicenseRouteGuardProps) => {
  const [isValid, setIsValid] = useState<boolean | null>(null);
  const [loading, setLoading] = useState(true);
  const location = useLocation();
  const navigate = useNavigate();

  useEffect(() => {
    const checkLicense = async () => {
      try {
        if (isAdminRoute) {
          // For admin routes, check admin license status
          try {
            const status = await licenseService.getLicenseStatus();
            setIsValid(status.isValid && !status.isExpired);
          } catch (error) {
            // If error (e.g., not authenticated), allow to proceed to login/license entry
            setIsValid(false);
          }
        } else if (isPublicRoute) {
          // For public routes, check public license status
          try {
            const status = await licenseService.getPublicLicenseStatus();
            setIsValid(status.isValid && !status.isExpired);
          } catch (error) {
            // If API fails (e.g., network error, 500), allow access to prevent blocking users
            // Only block if we explicitly get invalid license response
            console.warn('License check failed, allowing access:', error);
            setIsValid(true); // Allow access on API errors
          }
        } else {
          // For other routes, allow
          setIsValid(true);
        }
      } catch (error) {
        console.error('Error checking license:', error);
        setIsValid(false);
      } finally {
        setLoading(false);
      }
    };

    checkLicense();
  }, [isAdminRoute, isPublicRoute, location.pathname]);

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 dark:bg-gray-900">
        <div className="text-center">
          <Loader2 className="h-8 w-8 animate-spin mx-auto text-blue-600" />
          <p className="mt-4 text-gray-600 dark:text-gray-400">Yükleniyor...</p>
        </div>
      </div>
    );
  }

  // If license is invalid
  if (isValid === false) {
    if (isAdminRoute) {
      // For admin routes, redirect to license key entry page
      // But only if not already on license key entry page
      if (location.pathname !== '/admin/license-key') {
        navigate('/admin/license-key', { replace: true });
        return null;
      }
      // If already on license key entry page, allow it
      return <>{children}</>;
    } else if (isPublicRoute) {
      // For public routes, show maintenance page
      return <Maintenance />;
    }
  }

  // License is valid or route doesn't require license check
  return <>{children}</>;
};

export default LicenseRouteGuard;

