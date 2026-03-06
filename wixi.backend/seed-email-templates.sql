-- Seed initial email templates
-- These templates use placeholders like {{FirstName}}, {{LastName}}, etc.

-- Contact Form Template
IF NOT EXISTS (SELECT 1 FROM wixi_EmailTemplate WHERE [Key] = 'ContactForm')
BEGIN
    INSERT INTO wixi_EmailTemplate ([Key], Subject, BodyHtml, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (
        'ContactForm',
        '🆕 Neue Kontaktanfrage von {{FirstName}} {{LastName}}',
        '<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #2563eb; color: white; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background-color: #f9fafb; padding: 20px; }
        .section { background-color: white; margin: 15px 0; padding: 15px; border-radius: 6px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .section-title { color: #2563eb; font-weight: bold; margin-bottom: 10px; font-size: 16px; }
        .field { margin: 8px 0; }
        .label { font-weight: 600; color: #4b5563; }
        .value { color: #1f2937; }
        .footer { text-align: center; padding: 20px; color: #6b7280; font-size: 12px; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h2>🆕 Neue Kontaktanfrage</h2>
        </div>
        <div class="content">
            <div class="section">
                <div class="section-title">👤 Persönliche Informationen</div>
                <div class="field"><span class="label">Name:</span> <span class="value">{{FirstName}} {{LastName}}</span></div>
                <div class="field"><span class="label">E-Mail:</span> <span class="value">{{Email}}</span></div>
                <div class="field"><span class="label">Telefon:</span> <span class="value">{{Phone}}</span></div>
            </div>
            <div class="section">
                <div class="section-title">💬 Nachricht</div>
                <div class="field"><span class="value">{{Message}}</span></div>
            </div>
        </div>
        <div class="footer">
            <p>Worklines - Ihre Brücke zur Karriere in Deutschland</p>
            <p>Automatisch generierte E-Mail vom Kontaktformular</p>
        </div>
    </div>
</body>
</html>',
        'Template for contact form submissions',
        1,
        SYSUTCDATETIME(),
        SYSUTCDATETIME()
    );
END

-- Employer Form Template
IF NOT EXISTS (SELECT 1 FROM wixi_EmailTemplate WHERE [Key] = 'EmployerForm')
BEGIN
    INSERT INTO wixi_EmailTemplate ([Key], Subject, BodyHtml, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (
        'EmployerForm',
        '🏢 Neue Arbeitgeberanfrage von {{CompanyName}}',
        '<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #059669; color: white; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background-color: #f9fafb; padding: 20px; }
        .section { background-color: white; margin: 15px 0; padding: 15px; border-radius: 6px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .section-title { color: #059669; font-weight: bold; margin-bottom: 10px; font-size: 16px; }
        .field { margin: 8px 0; }
        .label { font-weight: 600; color: #4b5563; }
        .value { color: #1f2937; }
        .footer { text-align: center; padding: 20px; color: #6b7280; font-size: 12px; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h2>🏢 Neue Arbeitgeberanfrage</h2>
        </div>
        <div class="content">
            <div class="section">
                <div class="section-title">🏢 Unternehmensinformationen</div>
                <div class="field"><span class="label">Firmenname:</span> <span class="value">{{CompanyName}}</span></div>
                <div class="field"><span class="label">Ansprechpartner:</span> <span class="value">{{ContactPerson}}</span></div>
                <div class="field"><span class="label">E-Mail:</span> <span class="value">{{Email}}</span></div>
                <div class="field"><span class="label">Telefon:</span> <span class="value">{{Phone}}</span></div>
                <div class="field"><span class="label">Branche:</span> <span class="value">{{Industry}}</span></div>
            </div>
            <div class="section">
                <div class="section-title">👥 Stellenanforderungen</div>
                <div class="field"><span class="label">Gesuchte Positionen:</span> <span class="value">{{Positions}}</span></div>
                <div class="field"><span class="label">Anforderungen:</span><br><span class="value">{{Requirements}}</span></div>
            </div>
        </div>
        <div class="footer">
            <p>Worklines - Ihre Brücke zur Karriere in Deutschland</p>
            <p>Automatisch generierte E-Mail vom Arbeitgeberformular</p>
        </div>
    </div>
</body>
</html>',
        'Template for employer form submissions',
        1,
        SYSUTCDATETIME(),
        SYSUTCDATETIME()
    );
END

-- Employee Form Template
IF NOT EXISTS (SELECT 1 FROM wixi_EmailTemplate WHERE [Key] = 'EmployeeForm')
BEGIN
    INSERT INTO wixi_EmailTemplate ([Key], Subject, BodyHtml, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (
        'EmployeeForm',
        '👤 Neue Arbeitnehmerbewerbung von {{FullName}}',
        '<!DOCTYPE html>
<html>
<head>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #dc2626; color: white; padding: 20px; border-radius: 8px 8px 0 0; }
        .content { background-color: #f9fafb; padding: 20px; }
        .section { background-color: white; margin: 15px 0; padding: 15px; border-radius: 6px; box-shadow: 0 1px 3px rgba(0,0,0,0.1); }
        .section-title { color: #dc2626; font-weight: bold; margin-bottom: 10px; font-size: 16px; }
        .field { margin: 8px 0; }
        .label { font-weight: 600; color: #4b5563; }
        .value { color: #1f2937; }
        .footer { text-align: center; padding: 20px; color: #6b7280; font-size: 12px; }
    </style>
</head>
<body>
    <div class="container">
        <div class="header">
            <h2>👤 Neue Arbeitnehmerbewerbung</h2>
        </div>
        <div class="content">
            <div class="section">
                <div class="section-title">👤 Persönliche Informationen</div>
                <div class="field"><span class="label">Name:</span> <span class="value">{{FullName}}</span></div>
                <div class="field"><span class="label">E-Mail:</span> <span class="value">{{Email}}</span></div>
                <div class="field"><span class="label">Telefon:</span> <span class="value">{{Phone}}</span></div>
            </div>
            <div class="section">
                <div class="section-title">🎓 Qualifikationen</div>
                <div class="field"><span class="label">Beruf:</span> <span class="value">{{Profession}}</span></div>
                <div class="field"><span class="label">Berufserfahrung:</span> <span class="value">{{Experience}} Jahre</span></div>
                <div class="field"><span class="label">Deutschkenntnisse:</span> <span class="value">{{GermanLevel}}</span></div>
            </div>
        </div>
        <div class="footer">
            <p>Worklines - Ihre Brücke zur Karriere in Deutschland</p>
            <p>Automatisch generierte E-Mail vom Arbeitnehmerformular</p>
        </div>
    </div>
</body>
</html>',
        'Template for employee form submissions',
        1,
        SYSUTCDATETIME(),
        SYSUTCDATETIME()
    );
END

-- Client Code Template (for new client registration)
IF NOT EXISTS (SELECT 1 FROM wixi_EmailTemplate WHERE [Key] = 'ClientCode')
BEGIN
    INSERT INTO wixi_EmailTemplate ([Key], Subject, BodyHtml, Description, IsActive, CreatedAt, UpdatedAt)
    VALUES (
        'ClientCode',
        'Willkommen bei Worklines - Ihr Kundenkode: {{ClientCode}}',
        '<!DOCTYPE html>
<html>
<head>
    <meta charset=''utf-8''>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #2563eb; color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }
        .content { background-color: #f9fafb; padding: 20px; border: 1px solid #e5e7eb; }
        .code-box { background-color: white; border: 2px solid #2563eb; padding: 15px; margin: 20px 0; text-align: center; border-radius: 5px; }
        .code { font-size: 24px; font-weight: bold; color: #2563eb; letter-spacing: 2px; }
        .footer { margin-top: 20px; padding-top: 20px; border-top: 1px solid #e5e7eb; font-size: 12px; color: #6b7280; }
        .warning { background-color: #fef3c7; border-left: 4px solid #f59e0b; padding: 10px; margin: 15px 0; }
        .button { background-color: #2563eb; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block; margin: 10px 0; }
        .info { background-color: #dbeafe; border-left: 4px solid #2563eb; padding: 10px; margin: 15px 0; }
    </style>
</head>
<body>
    <div class=''container''>
        <div class=''header''>
            <h1>Willkommen bei Worklines</h1>
        </div>
        <div class=''content''>
            <p>Sehr geehrte/r {{FullName}},</p>
            
            <p>Vielen Dank für Ihr Interesse an unseren Dienstleistungen. Wir haben Ihnen einen persönlichen Kundenkode erstellt.</p>
            
            <p><strong>Ihr Kundenkode:</strong></p>
            <div class=''code-box''>
                <div class=''code''>{{ClientCode}}</div>
            </div>
            
            <div class=''warning''>
                <strong>⚠️ Wichtig:</strong> Dieser Kode ist <strong>3 Tage gültig</strong> (bis {{ExpirationDate}} Uhr).
                Bitte verwenden Sie ihn innerhalb dieser Zeit, um Ihr Konto zu registrieren.
            </div>
            
            <div class=''info''>
                <strong>📋 So geht''s weiter:</strong>
                <ol style=''margin: 10px 0; padding-left: 20px;''>
                    <li>Klicken Sie auf den untenstehenden Button, um zur Registrierung zu gelangen</li>
                    <li>Geben Sie Ihren Kundenkode bei der Registrierung ein</li>
                    <li>Vervollständigen Sie Ihr Profil und laden Sie die erforderlichen Dokumente hoch</li>
                </ol>
            </div>
            
            <p style=''text-align: center;''>
                <a href=''{{RegisterUrl}}'' class=''button''>Jetzt registrieren</a>
            </p>
            
            <p style=''font-size: 12px; color: #6b7280; text-align: center;''>
                Oder besuchen Sie: <a href=''{{RegisterUrl}}''>{{RegisterUrl}}</a>
            </p>
            
            <p>Mit diesem Kode können Sie sich in unserem System registrieren und Ihre Bewerbung fortsetzen.</p>
            
            <p>Bei Fragen stehen wir Ihnen gerne zur Verfügung.</p>
            
            <p>Mit freundlichen Grüßen,<br>Das Worklines Team</p>
        </div>
        <div class=''footer''>
            <p>Worklines - Ihr Partner für Bildung und Karriere in Deutschland</p>
            <p>Diese E-Mail wurde automatisch generiert. Bitte antworten Sie nicht direkt auf diese E-Mail.</p>
        </div>
    </div>
</body>
</html>',
        'Template for client code email sent to new clients',
        1,
        SYSUTCDATETIME(),
        SYSUTCDATETIME()
    );
END

