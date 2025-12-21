# 📸 Frontend Integration Guide - Image Management API

## Quick Start

### Step 1: Upload Image
```javascript
// Upload image to Cloudinary via API
const uploadImage = async (file) => {
  const formData = new FormData();
  formData.append('file', file);

  const response = await fetch('/api/Upload/photo', {
    method: 'POST',
    body: formData,
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });

  const { url, publicId } = await response.json();
  return { url, publicId };
};
```

### Step 2: Store in Entity
```javascript
// When creating a Trip, Hotel, Car, or Attraction
const createTrip = async (tripData, imageUrl, imagePublicId) => {
  const payload = {
    ...tripData,
    mainImage: imageUrl,  // Single main image
    images: [imageUrl],    // Array of image URLs
    // Important: Store publicId for future deletion
    _imagePublicIds: [imagePublicId]  // Optional: custom field for tracking
  };

  await fetch('/api/Trips', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(payload)
  });
};
```

### Step 3: Delete Image (Optional)
```javascript
// When deleting an entity or replacing an image
const deleteImage = async (publicId) => {
  const response = await fetch('/api/Upload/delete', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify({ publicId })
  });

  const { success } = await response.json();
  return success;
};
```

---

## API Endpoints

### Upload Photo
**Endpoint**: `POST /api/Upload/photo`

**Request**:
```
Content-Type: multipart/form-data

Field: file (binary image file)
Max Size: 100MB
Formats: JPEG, PNG, GIF, WebP
```

**Response** (200 OK):
```json
{
  "url": "https://res.cloudinary.com/mycloud/image/upload/v1234567890/rentify/image-001.jpg",
  "publicId": "rentify/image-001"
}
```

**Error Response** (400 Bad Request):
```json
{
  "error": "No file provided or file is empty."
}
```

---

### Delete Photo
**Endpoint**: `POST /api/Upload/delete`

**Request**:
```json
{
  "publicId": "rentify/image-001"
}
```

**Response** (200 OK):
```json
{
  "success": true,
  "message": "Image deleted successfully."
}
```

**Error Response** (400 Bad Request):
```json
{
  "error": "PublicId is required."
}
```

---

## Best Practices

### ✅ DO
- ✅ Always upload the image **before** creating/updating an entity
- ✅ Store **both** `url` and `publicId` in your database or state
- ✅ Use the `url` to display the image
- ✅ Use the `publicId` when you need to delete or replace the image
- ✅ Handle upload errors gracefully with user-friendly messages
- ✅ Show upload progress to the user (percentage complete)
- ✅ Validate file type and size on the client before uploading

### ❌ DON'T
- ❌ Don't delete the image from Cloudinary if it's still in use in the database
- ❌ Don't store the entire Cloudinary URL as the primary reference (use publicId)
- ❌ Don't upload without waiting for the response
- ❌ Don't forget to handle network errors and timeouts
- ❌ Don't allow users to delete images while the deletion is in progress

---

## Common Scenarios

### Update Trip with New Image
```javascript
// 1. Upload new image
const { url, publicId } = await uploadImage(newFile);

// 2. Delete old image (if replacing)
if (oldPublicId) {
  await deleteImage(oldPublicId);
}

// 3. Update trip with new image URL
await fetch(`/api/Trips/${tripId}`, {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    ...tripData,
    mainImage: url,
    images: [url]
  })
});
```

### Gallery with Multiple Images
```javascript
// Upload multiple images
const imageUrls = [];
const imagePublicIds = [];

for (const file of files) {
  const { url, publicId } = await uploadImage(file);
  imageUrls.push(url);
  imagePublicIds.push(publicId);
}

// Create attraction with all images
await createAttraction({
  ...attractionData,
  images: imageUrls  // Array of URLs
  // Store publicIds for future deletion management
});
```

### Handle Upload Progress
```javascript
const uploadImageWithProgress = async (file, onProgress) => {
  const formData = new FormData();
  formData.append('file', file);

  return new Promise((resolve, reject) => {
    const xhr = new XMLHttpRequest();

    xhr.upload.addEventListener('progress', (e) => {
      const percentComplete = (e.loaded / e.total) * 100;
      onProgress(percentComplete);
    });

    xhr.addEventListener('load', () => {
      if (xhr.status === 200) {
        resolve(JSON.parse(xhr.responseText));
      } else {
        reject(new Error(`Upload failed with status ${xhr.status}`));
      }
    });

    xhr.addEventListener('error', () => {
      reject(new Error('Upload failed'));
    });

    xhr.open('POST', '/api/Upload/photo');
    xhr.setRequestHeader('Authorization', `Bearer ${token}`);
    xhr.send(formData);
  });
};

// Usage
uploadImageWithProgress(file, (percent) => {
  console.log(`Upload progress: ${percent}%`);
  updateProgressBar(percent);
});
```

---

## Error Handling

### Client-Side Validation
```javascript
const validateImage = (file) => {
  const MAX_SIZE = 100 * 1024 * 1024; // 100MB
  const ALLOWED_TYPES = ['image/jpeg', 'image/png', 'image/gif', 'image/webp'];

  if (!file) {
    return { valid: false, error: 'No file selected' };
  }

  if (file.size > MAX_SIZE) {
    return { valid: false, error: `File too large. Max size: 100MB` };
  }

  if (!ALLOWED_TYPES.includes(file.type)) {
    return { valid: false, error: 'Invalid file type. Use JPEG, PNG, GIF, or WebP' };
  }

  return { valid: true };
};
```

### Server Error Handling
```javascript
const handleUploadError = async (error) => {
  if (error.status === 400) {
    const { error: message } = await error.json();
    showError(`Validation error: ${message}`);
  } else if (error.status === 500) {
    showError('Server error. Please try again later.');
  } else {
    showError('Network error. Please check your connection.');
  }
};
```

---

## Data Structure Examples

### Trip with Images
```json
{
  "id": 1,
  "title": "Paris Adventure",
  "mainImage": "https://res.cloudinary.com/mycloud/image/upload/v1234567890/trips/paris-main.jpg",
  "images": [
    "https://res.cloudinary.com/mycloud/image/upload/v1234567890/trips/paris-001.jpg",
    "https://res.cloudinary.com/mycloud/image/upload/v1234567890/trips/paris-002.jpg"
  ]
}
```

### Attraction with Images
```json
{
  "id": 1,
  "name": "Eiffel Tower",
  "images": [
    {
      "url": "https://res.cloudinary.com/mycloud/image/upload/v1234567890/attractions/eiffel-001.jpg"
    },
    {
      "url": "https://res.cloudinary.com/mycloud/image/upload/v1234567890/attractions/eiffel-002.jpg"
    }
  ]
}
```

---

## React Component Example

```jsx
import { useState } from 'react';

export const ImageUploader = ({ onImageUploaded }) => {
  const [uploading, setUploading] = useState(false);
  const [progress, setProgress] = useState(0);
  const [error, setError] = useState(null);

  const handleFileSelect = async (e) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setUploading(true);
    setError(null);
    setProgress(0);

    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await fetch('/api/Upload/photo', {
        method: 'POST',
        body: formData,
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        }
      });

      if (!response.ok) {
        throw new Error('Upload failed');
      }

      const { url, publicId } = await response.json();
      onImageUploaded({ url, publicId });
      setProgress(100);
    } catch (err) {
      setError(err.message);
    } finally {
      setUploading(false);
    }
  };

  return (
    <div>
      <input
        type="file"
        accept="image/*"
        onChange={handleFileSelect}
        disabled={uploading}
      />
      {uploading && <div>Upload progress: {progress}%</div>}
      {error && <div className="error">{error}</div>}
    </div>
  );
};
```

---

## Automatic Cleanup

**System-wide image cleanup runs automatically every hour**:
- Identifies images in Cloudinary not referenced in the database
- Automatically deletes orphaned images
- Logs all cleanup operations
- No manual intervention needed

**You don't need to worry about cleanup unless you manually delete images!**

---

## Support & Troubleshooting

### Common Issues

**Q: Image uploaded but not showing?**
- A: Make sure you're using the `url` field from the response, not the publicId

**Q: Delete endpoint returns error?**
- A: Verify the publicId is correct and the image still exists in Cloudinary

**Q: Image still in use but want to delete?**
- A: First delete/update all entities using that image, then delete from Cloudinary

**Q: Upload fails with 500 error?**
- A: Check Cloudinary credentials in server configuration

---

## Additional Resources

- **Cloudinary Documentation**: https://cloudinary.com/documentation
- **Image Transformations**: The API automatically applies 500x500 resize with face detection
- **Supported Formats**: JPEG, PNG, GIF, WebP
- **API Documentation**: Swagger/Scalar available at `/swagger` endpoint
