import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { LanguageProvider } from './contexts/language-context';
import { Toaster } from './components/ui/toaster';
import { useMemo } from 'react';
import { isProd } from './utils/env';
import Maintenance from './pages/Maintenance';
import Home from './pages/Home';
import About from './pages/About';
import Contact from './pages/Contact';
import Work from './pages/services/Work';
import Ausbildung from './pages/services/Ausbildung';
import Language from './pages/services/Language';
import University from './pages/services/University';
import Team from './pages/Team';
import TeamMember from './pages/TeamMember';
import NewsDetail from './pages/NewsDetail';
import FurArbeitgeber from './pages/FurArbeitgeber';
import FurArbeitnehmer from './pages/FurArbeitnehmer';
import NotFound from './pages/NotFound';
import Privacy from './pages/Privacy';
import Terms from './pages/Terms';
import Impressum from './pages/Impressum';
import FAQ from './pages/FAQ';
import Appointment from './pages/Appointment';
import References from './pages/References';

// Auth imports
import Login from './components/admin/Login';
import ForgotPassword from './components/admin/ForgotPassword';
import Register from './pages/Register';
import ProtectedRoute from './components/admin/ProtectedRoute';
import AdminLayout from './components/layouts/AdminLayout';
import Dashboard from './components/admin/Dashboard';
import Users from './components/admin/Users';
import Reports from './components/admin/Reports';
import SmtpSettings from './components/admin/SmtpSettings';
import Settings from './components/admin/Settings';
import ContentManagement from './components/admin/ContentManagement';
import EmailLogs from './components/admin/EmailLogs';
import ApplicationLogs from './components/admin/ApplicationLogs';
import EmailTemplates from './components/admin/EmailTemplates';
import EmployeeSubmissions from './components/admin/EmployeeSubmissions';
import EmployerSubmissions from './components/admin/EmployerSubmissions';
import ContactSubmissions from './components/admin/ContactSubmissions';
import TeamMembers from './components/admin/TeamMembers';
import News from './components/admin/News';
import Translations from './components/admin/Translations';
import Roles from './components/admin/Roles';
import ClientTracking from './components/admin/ClientTracking';
import SupportManagement from './components/admin/SupportManagement';
import FAQManagement from './components/admin/FAQManagement';
import ApplicationManagement from './components/admin/ApplicationManagement';
import DocumentReview from './components/admin/DocumentReview';
import ApplicationTemplateManagement from './components/admin/ApplicationTemplateManagement';
import DocumentTypeManagement from './components/admin/DocumentTypeManagement';
import AppointmentManagement from './pages/Admin/AppointmentManagement';
import AppointmentList from './pages/Admin/AppointmentList';
import HolidayManagement from './pages/Admin/HolidayManagement';
import PaymentList from './pages/Admin/PaymentList';
import MenuPermissionManagement from './pages/Admin/MenuPermissionManagement';
import TestimonialManagement from './pages/Admin/TestimonialManagement';
import ProjectReferenceManagement from './pages/Admin/ProjectReferenceManagement';
import EquivalencyFeeSettingsManagement from './pages/Admin/EquivalencyFeeSettingsManagement';
import LicenseKeyEntry from './pages/Admin/LicenseKeyEntry';
import LicenseRouteGuard from './components/LicenseRouteGuard';

// Client imports
import ClientLayout from './components/layouts/ClientLayout';
import ClientDashboard from './pages/Client/Dashboard';
import ClientProfile from './pages/Client/Profile';
import ClientDocuments from './pages/Client/Documents';
import ClientSupport from './pages/Client/Support';
import CVBuilder from './pages/Client/CVBuilder';
import CVBuilderPayment from './pages/Client/CVBuilderPayment';
import EquivalencyFeePayment from './pages/EquivalencyFeePayment';

function App() {
  const showPreviewOnlyFeatures = !isProd;
  // Domain kontrolü - worklines.de için maintenance modu
  const isMaintenanceMode = useMemo(() => {
    if (typeof window !== 'undefined') {
      const hostname = window.location.hostname.toLowerCase();
      // worklines.de ve tüm alt domain'leri için maintenance modu
      const isWorklinesDomain = hostname === 'worklines.de' || 
                                hostname === 'www.worklines.de' || 
                                hostname.endsWith('.worklines.de');
      return isWorklinesDomain;
    }
    return false;
  }, []);

  // Eğer maintenance modu aktifse, sadece maintenance sayfasını göster
  if (isMaintenanceMode) {
    return (
      <LanguageProvider>
        <Maintenance />
      </LanguageProvider>
    );
  }

  return (
    <LanguageProvider>
      <Router>
        <Routes>
          {/* Public Routes */}
          <Route path="/" element={<LicenseRouteGuard isPublicRoute><Home /></LicenseRouteGuard>} />
          <Route path="about" element={<LicenseRouteGuard isPublicRoute><About /></LicenseRouteGuard>} />
          <Route path="contact" element={<LicenseRouteGuard isPublicRoute><Contact /></LicenseRouteGuard>} />
          <Route path="services/work" element={<LicenseRouteGuard isPublicRoute><Work /></LicenseRouteGuard>} />
          <Route path="services/ausbildung" element={<LicenseRouteGuard isPublicRoute><Ausbildung /></LicenseRouteGuard>} />
          <Route path="services/language" element={<LicenseRouteGuard isPublicRoute><Language /></LicenseRouteGuard>} />
          <Route path="services/university" element={<LicenseRouteGuard isPublicRoute><University /></LicenseRouteGuard>} />
          <Route path="team" element={<LicenseRouteGuard isPublicRoute><Team /></LicenseRouteGuard>} />
          <Route path="team/:slug" element={<LicenseRouteGuard isPublicRoute><TeamMember /></LicenseRouteGuard>} />
          <Route path="news/:slug" element={<LicenseRouteGuard isPublicRoute><NewsDetail /></LicenseRouteGuard>} />
          <Route path="fur-arbeitgeber" element={<LicenseRouteGuard isPublicRoute><FurArbeitgeber /></LicenseRouteGuard>} />
          <Route path="fur-arbeitnehmer" element={<LicenseRouteGuard isPublicRoute><FurArbeitnehmer /></LicenseRouteGuard>} />
          <Route path="privacy" element={<LicenseRouteGuard isPublicRoute><Privacy /></LicenseRouteGuard>} />
          <Route path="terms" element={<LicenseRouteGuard isPublicRoute><Terms /></LicenseRouteGuard>} />
          <Route path="impressum" element={<LicenseRouteGuard isPublicRoute><Impressum /></LicenseRouteGuard>} />
          <Route path="faq" element={<LicenseRouteGuard isPublicRoute><FAQ /></LicenseRouteGuard>} />
          {showPreviewOnlyFeatures && (
            <>
              <Route path="appointment" element={<LicenseRouteGuard isPublicRoute><Appointment /></LicenseRouteGuard>} />
              <Route path="references" element={<LicenseRouteGuard isPublicRoute><References /></LicenseRouteGuard>} />
            </>
          )}

          {/* Auth Routes */}
          <Route path="/login" element={<Login />} />
          <Route path="/forgot-password" element={<ForgotPassword />} />
          {showPreviewOnlyFeatures && (
            <Route path="/register" element={<Register />} />
          )}

          {/* Admin Protected Routes */}
          <Route
            path="/admin"
            element={
              <LicenseRouteGuard isAdminRoute>
                <ProtectedRoute allowedRoles={['Admin']}>
                  <AdminLayout />
                </ProtectedRoute>
              </LicenseRouteGuard>
            }
          >
            <Route index element={<Dashboard />} />
            <Route path="dashboard" element={<Dashboard />} />
            <Route path="users" element={<Users />} />
            <Route path="reports" element={<Reports />} />
            <Route path="settings" element={<Settings />} />
            <Route path="content-management" element={<ContentManagement />} />
            <Route path="smtp-settings" element={<SmtpSettings />} />
            <Route path="email-logs" element={<EmailLogs />} />
            <Route path="email-templates" element={<EmailTemplates />} />
            <Route path="employee-submissions" element={<EmployeeSubmissions />} />
            <Route path="employer-submissions" element={<EmployerSubmissions />} />
            <Route path="contact-submissions" element={<ContactSubmissions />} />
            <Route path="team-members" element={<TeamMembers />} />
            <Route path="news" element={<News />} />
            <Route path="application-logs" element={<ApplicationLogs />} />
            <Route path="translations" element={<Translations />} />
            <Route path="roles" element={<Roles />} />
            <Route path="client-tracking" element={<ClientTracking />} />
            <Route path="support-management" element={<SupportManagement />} />
            <Route path="faq-management" element={<FAQManagement />} />
            <Route path="application-management" element={<ApplicationManagement />} />
            <Route path="application-templates" element={<ApplicationTemplateManagement />} />
            <Route path="document-types" element={<DocumentTypeManagement />} />
            <Route path="document-review" element={<DocumentReview />} />
            {showPreviewOnlyFeatures && (
              <>
                <Route path="appointments" element={<AppointmentManagement />} />
                <Route path="appointments-list" element={<AppointmentList />} />
              </>
            )}
            <Route path="holidays" element={<HolidayManagement />} />
            <Route path="payments" element={<PaymentList />} />
            <Route path="testimonials" element={<TestimonialManagement />} />
            <Route path="project-references" element={<ProjectReferenceManagement />} />
            <Route path="equivalency-fee-settings" element={<EquivalencyFeeSettingsManagement />} />
            <Route path="menu-permissions" element={<MenuPermissionManagement />} />
            <Route path="license-key" element={<LicenseKeyEntry />} />
          </Route>

          {/* Client Protected Routes */}
          <Route
            path="/client"
            element={
              <ProtectedRoute allowedRoles={['Client', 'Danisan', 'Danışan']}>
                <ClientLayout />
              </ProtectedRoute>
            }
          >
            <Route path="dashboard" element={<ClientDashboard />} />
            <Route path="profile" element={<ClientProfile />} />
            <Route path="documents" element={<ClientDocuments />} />
            <Route path="support" element={<ClientSupport />} />
            <Route path="cv-builder" element={<CVBuilder />} />
            <Route path="cv-builder-payment" element={<CVBuilderPayment />} />
            <Route path="equivalency-fee-payment" element={<EquivalencyFeePayment />} />
          </Route>

          {/* 404 */}
          <Route path="*" element={<NotFound />} />
        </Routes>
      </Router>
      <Toaster />
      <ToastContainer
        position="top-right"
        autoClose={3000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="light"
      />
    </LanguageProvider>
  );
}

export default App;
