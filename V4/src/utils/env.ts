const inferEnvFromHostname = (): 'production' | 'development' | undefined => {
  if (typeof window === 'undefined') {
    return undefined;
  }

  const hostname = window.location.hostname.toLowerCase();

  if (hostname === 'worklines.com.tr' || hostname === 'www.worklines.com.tr' ||
      hostname === 'worklines.de' || hostname === 'www.worklines.de') {
    return 'production';
  }

  if (
    hostname === 'test.worklines.com.tr' ||
    hostname === 'www.test.worklines.com.tr'
  ) {
    return 'development';
  }

  return undefined;
};

const inferredEnv = inferEnvFromHostname();
export const appEnv =
  (import.meta.env.VITE_APP_ENV as 'production' | 'development' | undefined) ??
  inferredEnv ??
  'development';

export const isProd = appEnv === 'production';
export const isDev = !isProd;

