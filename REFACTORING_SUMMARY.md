# Blood Donation Support System - Image Upload Refactoring

## Summary of Changes

### ??? **Removed Cloudinary Integration**
- Removed `ICloudinaryService` and `CloudinaryService` from dependency injection
- Removed Cloudinary configuration from `appsettings.json`
- Removed Cloudinary package dependencies from Program.cs

### ??? **New Simple Image Controller**
Created a simplified `ImageController` that only handles image URL storage:

#### **Available Endpoints:**
1. **`POST /api/image/store-url`** - Store single image URL
2. **`POST /api/image/store-multiple-urls`** - Store multiple image URLs
3. **`POST /api/image/validate-url`** - Validate image URL accessibility

#### **Features:**
- ? URL format validation
- ? HTTP/HTTPS scheme validation
- ? URL accessibility checking (optional)
- ? Authorization required for storage endpoints
- ? Support for batch URL operations (max 10 URLs)

### ?? **Enhanced Article System**

#### **Updated DTOs:**
- **`ArticleDTO`** - Enhanced with validation attributes
- **`ArticleCreateDTO`** - New DTO for article creation (without AuthorUserId)
- **`ArticleResponseDTO`** - New DTO for API responses
- **`UpdateArticleDTO`** - Enhanced with validation attributes

#### **Validation Features:**
- ? Required field validation
- ? String length validation (Title max 500 chars)
- ? URL validation for Picture field
- ? Model state validation in controllers

#### **Enhanced ArticleController:**
- ? Better error handling with standardized responses
- ? Pagination support for article listing
- ? Role-based authorization (Admin/Staff for CUD operations)
- ? Automatic author assignment from JWT token
- ? Filtering by category and status
- ? Author-specific article access control

### ?? **Security Improvements**
- ? Proper authorization for image storage
- ? Role-based access control for articles
- ? Input validation and sanitization
- ? URL format and accessibility validation

### ?? **Usage Guide**

#### **For Frontend Developers:**

1. **Upload Images to Cloud Service** (Frontend responsibility):
   ```javascript
   // Upload image to Cloudinary/AWS S3/etc. using frontend
   const imageUrl = await uploadToCloudService(imageFile);
   ```

2. **Store Image URL via API**:
   ```javascript
   // Store the URL in backend
   const response = await fetch('/api/image/store-url', {
     method: 'POST',
     headers: {
       'Authorization': `Bearer ${token}`,
       'Content-Type': 'application/json'
     },
     body: JSON.stringify({
       imageUrl: imageUrl,
       description: 'Article featured image',
       category: 'article'
     })
   });
   ```

3. **Create Article with Image URL**:
   ```javascript
   const articleData = {
     articleCategoryId: 1,
     articleStatusId: 1,
     title: 'Article Title',
     content: 'Article content...',
     picture: imageUrl // Use the URL from step 1
   };

   const response = await fetch('/api/articles', {
     method: 'POST',
     headers: {
       'Authorization': `Bearer ${token}`,
       'Content-Type': 'application/json'
     },
     body: JSON.stringify(articleData)
   });
   ```

#### **Article API Endpoints:**

| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/articles` | Get all articles (with pagination) | Public |
| GET | `/api/articles/{id}` | Get article by ID | Public |
| POST | `/api/articles` | Create new article | Admin/Staff |
| PUT | `/api/articles/{id}` | Update article | Admin/Staff |
| DELETE | `/api/articles/{id}` | Delete article | Admin/Staff |
| GET | `/api/articles/author/{authorId}` | Get articles by author | Authorized |
| GET | `/api/articles/category/{categoryId}` | Get articles by category | Public |
| GET | `/api/articles/status/{statusId}` | Get articles by status | Role-based |

#### **Query Parameters for Article Listing:**
- `page` - Page number (default: 1)
- `pageSize` - Items per page (default: 10, max: 50)
- `categoryId` - Filter by category
- `statusId` - Filter by status

### ? **Benefits of New Architecture**

1. **?? Decoupled Architecture**: Frontend handles file uploads, backend handles business logic
2. **?? Cloud Provider Agnostic**: Can use any cloud service (Cloudinary, AWS S3, etc.)
3. **? Better Performance**: No server-side file processing
4. **??? Enhanced Security**: Proper validation and authorization
5. **?? Scalable**: Supports multiple image formats and providers
6. **?? Focused Responsibility**: Each layer has clear responsibilities

### ?? **Configuration Required**

Update your User Secrets with basic configuration:
```bash
dotnet user-secrets set "AppSetting:SecretKey" "YourSecretKey32CharactersLong"
dotnet user-secrets set "ConnectionStrings:BloodDonationDB" "YourConnectionString"
```

### ?? **Database Schema**
The existing `Article` entity remains unchanged:
- `Picture` field stores image URL (string)
- `Content` field stores article content (string)
- All other fields remain as before

This refactoring provides a cleaner, more maintainable, and scalable solution for image handling in your Blood Donation Support System! ??