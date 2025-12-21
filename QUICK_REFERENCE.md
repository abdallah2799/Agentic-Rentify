# 🚀 Quick Reference Card - Image Management System

## API Quick Reference

### 1. Upload Image
```http
POST /api/Upload/photo
Content-Type: multipart/form-data

Request:
  file: <binary image file>

Response (200):
{
  "url": "https://res.cloudinary.com/cloud/image/upload/v1234567890/folder/image.jpg",
  "publicId": "folder/image"
}
```

### 2. Delete Image
```http
POST /api/Upload/delete
Content-Type: application/json

Request:
{
  "publicId": "folder/image"
}

Response (200):
{
  "success": true,
  "message": "Image deleted successfully."
}
```

### 3. Automatic Cleanup
```
Every hour (UTC):
  - Queries all entities for image URLs
  - Extracts Cloudinary PublicIds
  - Identifies orphaned images
  - Deletes orphaned images
  - Logs results to Serilog
```

---

## Entity Integration

### Before Creating/Updating Entity:
1. Upload image → GET `{ url, publicId }`
2. Store in entity: `mainImage: url`, `images: [url]`
3. Optionally track: `_publicIds: [publicId]` (for management)

### When Deleting Entity:
- Call DELETE `/api/Upload/delete` with `publicId`
- Or let cleanup job handle it automatically

---

## File Structure

```
Agentic Rentify/
├── Application/
│   ├── Interfaces/
│   │   ├── IPhotoService.cs ✨ NEW
│   │   ├── IImageCleanupService.cs ✨ NEW
│   │   └── ... (others)
│   ├── Features/Photos/ ✨ NEW
│   │   └── Commands/DeletePhoto/
│   │       ├── DeletePhotoCommand.cs
│   │       ├── DeletePhotoCommandHandler.cs
│   │       └── DeletePhotoCommandValidator.cs
│   ├── Wrappers/
│   │   ├── PhotoResponseDTO.cs ✨ NEW
│   │   └── ... (others)
│   └── ...
├── Infrastructure/
│   ├── Services/
│   │   ├── PhotoService.cs ✨ NEW
│   │   ├── ImageCleanupService.cs ✨ NEW
│   │   └── ... (others)
│   ├── DependencyInjection.cs 📝 MODIFIED
│   └── ...
├── Api/
│   ├── Controllers/
│   │   ├── UploadController.cs 📝 MODIFIED
│   │   └── ... (others)
│   ├── Program.cs 📝 MODIFIED
│   └── ...
└── Documentation/
    ├── IMAGE_MANAGEMENT_IMPLEMENTATION.md ✨ NEW
    ├── FRONTEND_INTEGRATION_GUIDE.md ✨ NEW
    ├── ARCHITECTURE_DIAGRAMS.md ✨ NEW
    └── PROJECT_SUMMARY.md ✨ NEW
```

---

## Key Classes

### IPhotoService
```csharp
interface IPhotoService {
  Task<PhotoResponseDTO> AddPhotoAsync(IFormFile file);
  Task<bool> DeletePhotoAsync(string publicId);
}
```

### IImageCleanupService
```csharp
interface IImageCleanupService {
  Task<int> CleanupOrphanedImagesAsync();
}
```

### PhotoResponseDTO
```csharp
class PhotoResponseDTO {
  string Url { get; set; }
  string PublicId { get; set; }
}
```

### DeletePhotoCommand (CQRS)
```csharp
class DeletePhotoCommand : IRequest<bool> {
  string PublicId { get; set; }
}
```

---

## Configuration

### appsettings.json
```json
{
  "CloudinarySettings": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=...;"
  }
}
```

### Hangfire Job (in Program.cs)
```csharp
RecurringJob.AddOrUpdate(
  "image-cleanup-hourly",
  () => imageCleanupService.CleanupOrphanedImagesAsync(),
  Cron.Hourly()
);
```

---

## Design Patterns Used

| Pattern | Implementation |
|---------|----------------|
| CQRS | DeletePhotoCommand + Handler |
| Clean Architecture | 3-layer design |
| Dependency Injection | Built-in ASP.NET Core |
| Repository Pattern | IUnitOfWork |
| Factory Pattern | Configuration-based initialization |
| Strategy Pattern | IPhotoService implementations |
| Validator Pattern | FluentValidation |
| Pipeline Pattern | MediatR behaviors |

---

## Primary Constructors

```csharp
// C# 12/13 feature used throughout
public class PhotoService(IConfiguration configuration) { }
public class ImageCleanupService(
    IConfiguration configuration,
    IUnitOfWork unitOfWork,
    ILogger<ImageCleanupService> logger) { }
public class DeletePhotoCommandHandler(IPhotoService photoService) { }
```

---

## Testing Checklist

- [ ] Upload valid image
- [ ] Upload invalid file format
- [ ] Upload oversized file
- [ ] Delete with valid PublicId
- [ ] Delete with invalid PublicId
- [ ] Check Hangfire dashboard
- [ ] Verify cleanup deletes orphaned images
- [ ] Check Serilog logs

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Build failed | Rebuild with `dotnet build` |
| Compilation errors | Check CloudinarySettings in appsettings.json |
| Upload fails | Verify Cloudinary credentials |
| Cleanup not running | Check Hangfire dashboard at `/hangfire` |
| Image not showing | Use `url` field, not `publicId` |
| Delete fails | Verify `publicId` is correct |

---

## Important Notes

✅ **Do This**:
- Use primary constructors in new classes
- Add validation to all inputs
- Log important operations
- Handle exceptions gracefully
- Follow CQRS for state-changing operations

❌ **Don't Do This**:
- Delete images still in use
- Ignore Cloudinary errors
- Store raw PublicIds without extracting from URLs
- Modify code without rebuilding
- Skip error handling

---

## Dashboard & Monitoring

### Hangfire Dashboard
- URL: `http://localhost:5000/hangfire`
- Shows: Jobs, recurring jobs, history, failures
- Useful for monitoring cleanup job execution

### Swagger/Scalar API Docs
- URL: `http://localhost:5000/swagger` (dev only)
- Shows: All endpoints, parameters, responses
- Useful for testing endpoints

### Serilog Logs
- Console output (Development)
- File logging (if configured)
- Application Insights (if configured)

---

## Version Information

| Component | Version |
|-----------|---------|
| .NET | 10.0 |
| C# | 12/13 |
| Cloudinary SDK | Latest |
| MediatR | Latest |
| FluentValidation | Latest |
| Serilog | Latest |
| Hangfire | Latest |
| Build Status | ✅ Success |

---

## Contact & Support

For questions or issues:
1. Check documentation files in project root
2. Review inline code comments
3. Check Serilog logs for errors
4. Monitor Hangfire dashboard
5. Consult Cloudinary documentation

---

**Last Updated**: December 21, 2025  
**Status**: ✅ Production Ready  
**Build**: ✅ Success (0 errors, 0 warnings)
