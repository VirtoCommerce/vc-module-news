# Virto Commerce News Module
The News module enables content managers to publish news articles for selected user groups using different languages within the Virto Commerce platform. Additionally, the module offers features for scheduling articles publishing.

<img width="1902" height="754" alt="470339158-831f0e3e-8236-469c-98a2-0aeb0623b054" src="https://github.com/user-attachments/assets/8cb394fe-88ad-486f-a56e-6f488b9eb00f" />


## Key features

* Write articles using extended markup
* Localize articles to multiple languages
* Select multiple target user groups
* Shedule articles publishing
* Setup SEO data

## Screenshots
<img width="822" height="624" alt="image" src="https://github.com/user-attachments/assets/31e9dd00-b474-4336-b587-b14d3c8ac338" />

---

<img width="847" height="727" alt="image" src="https://github.com/user-attachments/assets/c452647a-455a-4518-b1a9-66edad94375f" /> 

## XAPI Specification

### Query
```js
{
  newsArticles(
    storeId: "B2B-store"
    languageCode: "en-US"
  ) {
    items {
      id
      title
      content
      contentPreview
      publishDate
      seoInfo {
        id
        name
        semanticUrl
        pageTitle
        metaDescription
        metaKeywords
      }
    }
  }
}
```
---
```js
{
  newsArticle(
    id: "4ae1bb12-fd8f-4dcf-be92-9d5ad50b2a62"
    storeId: "B2B-store"
    languageCode: "en-US"
  ) {
    id
    title
    content
    contentPreview
    publishDate
    seoInfo {
      id
      name
      semanticUrl
      pageTitle
      metaDescription
      metaKeywords
    }
  }
}
```

## Documentation 

## References 

## License
Copyright (c) Virto Solutions LTD.  All rights reserved.

This software is licensed under the Virto Commerce Open Software License (the "License"); you
may not use this file except in compliance with the License. You may
obtain a copy of the License at http://virtocommerce.com/opensourcelicense.

Unless required by the applicable law or agreed to in written form, the software
distributed under the License is provided on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
implied.
