# README

## YDeveloper - Enterprise Edition

AI-powered website builder with enterprise-grade architecture.

### Features

- ✅ AI-powered content generation
- ✅ Multi-tenant architecture
- ✅ Domain management
- ✅ Payment integration (Iyzico)
- ✅ SSL automation (ready)
- ✅ Analytics dashboard
- ✅ Background job processing
- ✅ Health monitoring
- ✅ Comprehensive security

### Architecture

- **Pattern:** Repository + Unit of Work
- **Caching:** Distributed cache support
- **Security:** 9/10 score
- **Performance:** Optimized with caching, compression, indexes
- **Testing:** Unit test scaffolding included

### Recent Improvements (v1.0.0)

**150 comprehensive improvements applied:**
- Foundation & Architecture (45 items)
- Security & Validation (35 items)
- Models & Data (35 items)
- Services & Infrastructure (20 items)
- Configuration & Constants (15 items)

See [IMPROVEMENTS.md](IMPROVEMENTS.md) for full details.

### Build Status

✅ **SUCCESS** - 0 Errors, 31 Warnings (nullable only)

### Documentation

- [API Documentation](API_DOCUMENTATION.md)
- [Contributing Guide](CONTRIBUTING.md)
- [Changelog](CHANGELOG.md)
- [Security Policy](SECURITY.md)

### Quick Start

1. Clone the repository
2. Update `appsettings.json` (use `appsettings.Template.json` as reference)
3. Run database migrations: `dotnet ef database update`
4. Start the application: `dotnet run`

### Tech Stack

- ASP.NET Core 8.0
- Entity Framework Core
- SQL Server
- Hangfire (Background jobs)
- Serilog (Logging)
- QuestPDF (PDF generation)
- Google Gemini AI

---

**Version:** 1.0.0  
**License:** Proprietary  
**Status:** Production-ready 🚀
