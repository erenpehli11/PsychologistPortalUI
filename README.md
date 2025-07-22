
# PsychologistPortalUI

**PsychologistPortalUI** is the ASP.NET Core MVC frontend for the Psychologist Website. It functions as the public-facing site, delivering dynamically managed content such as blog posts, case studies, services, and about section details — all sourced from the `PsychologistPortalAPI`.

---

## 🧩 Features

- 🔍 View dynamically loaded blogs and categories
- 🧠 Explore case studies grouped by category
- 💼 Showcase psychology services
- 🧑‍⚕️ Display "About the Psychologist" section
- ⚙️ Powered by REST API integration

---

## 📁 Project Structure

```plaintext
/Controllers
│
├── HomeController.cs              → Home page and error handling
├── BlogController.cs              → Blog list/detail views
├── BlogCategoryController.cs      → Blog category CRUD via AJAX
├── VakaController.cs              → Case study (vaka) list/detail views
├── VakaCategoryController.cs      → Case study categories AJAX management
├── HizmetController.cs            → Service display logic
├── AboutController.cs             → About page rendering and CRUD
```

---

## 🔌 API Integration

All content is fetched from the [PsychologistPortalAPI](https://github.com/your-org/PsychologistPortalAPI) using `HttpClient`. The base URL is defined via `appsettings.json` (`ApiBaseUrl`), enabling easy switching between environments.

> Example:
```json
{
  "ApiBaseUrl": "https://localhost:5014/api"
}
```

Each controller handles API calls and parses data into ViewModels for dynamic Razor rendering.

---

## 💡 Usage

### Running the Project Locally

1. Make sure the backend API (`PsychologistPortalAPI`) is up and running.
2. Configure the `ApiBaseUrl` in your `appsettings.json` or `appsettings.Development.json`.
3. Build and run the MVC project.

---

## 📞 Contact

For feedback or collaboration, please contact the repository owner.
