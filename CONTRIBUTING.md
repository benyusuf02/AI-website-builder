# Contributing to YDeveloper

## Code Standards

### C# Style Guide
- Use PascalCase for class names and public members
- Use camelCase for private fields with _ prefix
- Keep methods focused and under 50 lines
- Use async/await for all I/O operations
- Follow SOLID principles

### Architecture Patterns
- Repository pattern for data access
- Unit of Work for transactions
- DTOs for API responses
- Dependency Injection throughout

### Security
- Never commit secrets to repository
- Use appsettings.Template.json as reference
- All inputs must be validated
- All outputs must be sanitized

### Testing
- Write unit tests for new features
- Maintain >80% code coverage
- Test edge cases and error scenarios

### Pull Request Process
1. Create feature branch from main
2. Make changes with descriptive commits
3. Ensure all tests pass
4. Update documentation
5. Submit PR with clear description

## Questions?
Open an issue or contact the team.
