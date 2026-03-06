import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Alert, AlertDescription } from '@/components/ui/alert';
import { Loader2, Key, CheckCircle2, XCircle, AlertCircle } from 'lucide-react';
import licenseService from '@/ApiServices/services/LicenseService';

// This page doesn't use LanguageProvider to avoid API calls that require license

const LicenseKeyEntry = () => {
  const [licenseKey, setLicenseKey] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [checkingStatus, setCheckingStatus] = useState(true);
  const [currentLicenseStatus, setCurrentLicenseStatus] = useState<{
    isValid: boolean;
    isExpired: boolean;
    expireDate?: string;
    daysRemaining?: number;
  } | null>(null);
  const navigate = useNavigate();

  useEffect(() => {
    // Check current license status to display information
    const checkLicenseStatus = async () => {
      try {
        // Use admin endpoint to get detailed license information (it's AllowAnonymous)
        const status = await licenseService.getLicenseStatus();
        // Calculate isExpired from daysRemaining
        const isExpired = status.daysRemaining !== undefined && status.daysRemaining <= 0;
        setCurrentLicenseStatus({
          isValid: status.isValid && !isExpired,
          isExpired: isExpired,
          expireDate: status.expireDate,
          daysRemaining: status.daysRemaining
        });
        // Don't redirect - allow user to view/manage license even if valid
      } catch (err) {
        // If error, show license entry form
        console.log('License check failed, showing entry form');
        setCurrentLicenseStatus({ isValid: false, isExpired: true });
      } finally {
        setCheckingStatus(false);
      }
    };

    checkLicenseStatus();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(false);

    try {
      const result = await licenseService.validateLicense(licenseKey);

      if (result.success && result.data?.isValid) {
        setSuccess(true);
        // Update current license status
        setCurrentLicenseStatus({
          isValid: true,
          isExpired: false,
          expireDate: result.data.expireDate,
          daysRemaining: result.data.daysRemaining
        });
        // Clear the input
        setLicenseKey('');
        // Don't redirect - allow user to stay on the page
      } else {
        setError(result.message || 'Lisans anahtarı geçersiz veya süresi dolmuş');
      }
    } catch (err: any) {
      setError(err.message || 'Lisans doğrulanırken bir hata oluştu');
    } finally {
      setLoading(false);
    }
  };

  if (checkingStatus) {
    return (
      <div className="container mx-auto py-6 px-4 max-w-4xl">
        <Card>
          <CardContent className="py-12">
            <div className="text-center">
              <Loader2 className="h-8 w-8 animate-spin mx-auto text-blue-600" />
              <p className="mt-4 text-gray-600 dark:text-gray-400">Lisans durumu kontrol ediliyor...</p>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="container mx-auto py-6 px-4 max-w-4xl">
      <Card>
        <CardHeader>
          <div className="flex items-center gap-3">
            <div className="w-12 h-12 bg-blue-100 dark:bg-blue-900 rounded-full flex items-center justify-center">
              <Key className="h-6 w-6 text-blue-600 dark:text-blue-400" />
            </div>
            <div>
              <CardTitle className="text-2xl">Lisans Yönetimi</CardTitle>
              <CardDescription>
                {currentLicenseStatus?.isValid && !currentLicenseStatus?.isExpired
                  ? 'Mevcut lisansınız geçerli. Yeni bir lisans anahtarı girebilir veya mevcut lisansınızı görüntüleyebilirsiniz.'
                  : 'Sisteme erişmek için geçerli bir lisans anahtarı girmeniz gerekmektedir.'}
              </CardDescription>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {/* Current License Status */}
          {!checkingStatus && currentLicenseStatus && (
            <Alert className={`mb-4 ${currentLicenseStatus.isValid && !currentLicenseStatus.isExpired 
              ? 'border-green-500 bg-green-50 dark:bg-green-900/20' 
              : 'border-yellow-500 bg-yellow-50 dark:bg-yellow-900/20'}`}>
              {currentLicenseStatus.isValid && !currentLicenseStatus.isExpired ? (
                <>
                  <CheckCircle2 className="h-4 w-4 text-green-600" />
                  <AlertDescription className="text-green-800 dark:text-green-200">
                    <strong>Lisans Geçerli</strong>
                    {currentLicenseStatus.expireDate && (
                      <div className="mt-1 text-sm">
                        Bitiş Tarihi: {new Date(currentLicenseStatus.expireDate).toLocaleDateString('tr-TR')}
                        {currentLicenseStatus.daysRemaining !== undefined && currentLicenseStatus.daysRemaining > 0 && (
                          <span> ({currentLicenseStatus.daysRemaining} gün kaldı)</span>
                        )}
                      </div>
                    )}
                  </AlertDescription>
                </>
              ) : (
                <>
                  <XCircle className="h-4 w-4 text-yellow-600" />
                  <AlertDescription className="text-yellow-800 dark:text-yellow-200">
                    <strong>Lisans Geçersiz veya Süresi Dolmuş</strong>
                    <div className="mt-1 text-sm">Yeni bir lisans anahtarı girmeniz gerekmektedir.</div>
                  </AlertDescription>
                </>
              )}
            </Alert>
          )}

          {error && (
            <Alert variant="destructive" className="mb-4">
              <XCircle className="h-4 w-4" />
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          {success && (
            <Alert className="mb-4 border-green-500 bg-green-50 dark:bg-green-900/20">
              <CheckCircle2 className="h-4 w-4 text-green-600" />
              <AlertDescription className="text-green-800 dark:text-green-200">
                Lisans başarıyla doğrulandı ve kaydedildi!
              </AlertDescription>
            </Alert>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="licenseKey">Lisans Anahtarı</Label>
              <Input
                id="licenseKey"
                type="text"
                placeholder="XXXX-XXXX-XXXX-XXXX"
                value={licenseKey}
                onChange={(e) => setLicenseKey(e.target.value.toUpperCase())}
                disabled={loading || success}
                required
                className="font-mono text-center"
              />
            </div>

            <Button
              type="submit"
              className="w-full"
              disabled={loading || success || !licenseKey.trim()}
            >
              {loading ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Doğrulanıyor...
                </>
              ) : success ? (
                <>
                  <CheckCircle2 className="mr-2 h-4 w-4" />
                  Başarılı
                </>
              ) : (
                <>
                  <Key className="mr-2 h-4 w-4" />
                  Lisansı Doğrula
                </>
              )}
            </Button>
          </form>

          <div className="mt-6 pt-6 border-t">
            <Alert>
              <AlertCircle className="h-4 w-4" />
              <AlertDescription className="text-sm text-gray-600 dark:text-gray-400">
                Lisans anahtarınızı almak için sistem yöneticinizle iletişime geçin.
              </AlertDescription>
            </Alert>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

export default LicenseKeyEntry;

