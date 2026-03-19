using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Services;
using StudyHelperAPI.Models.Classroom;
using StudyHelperAPI.Services.Interfaces;

namespace StudyHelperAPI.Services;

 public class ClassroomService : IClassroomService
    {
        private readonly IConfiguration _configuration;
        private Google.Apis.Classroom.v1.ClassroomService _service;

        public ClassroomService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<Google.Apis.Classroom.v1.ClassroomService> GetServiceAsync()
        {
            if (_service != null) return _service;

            var scopes = new[]
            {
                Google.Apis.Classroom.v1.ClassroomService.Scope.ClassroomCoursesReadonly,
                Google.Apis.Classroom.v1.ClassroomService.Scope.ClassroomCourseworkStudentsReadonly,
                Google.Apis.Classroom.v1.ClassroomService.Scope.ClassroomCourseworkMeReadonly,
                Google.Apis.Classroom.v1.ClassroomService.Scope.ClassroomCourseworkmaterialsReadonly,
                Google.Apis.Classroom.v1.ClassroomService.Scope.ClassroomRostersReadonly
            };

            GoogleCredential credential;

            var serviceAccountJson = _configuration["Google:ServiceAccountJson"];
            var serviceAccountPath = _configuration["Google:ServiceAccountPath"];
            if (!string.IsNullOrWhiteSpace(serviceAccountJson) || !string.IsNullOrWhiteSpace(serviceAccountPath))
            {
                credential = !string.IsNullOrWhiteSpace(serviceAccountJson)
                    ? GoogleCredential.FromJson(serviceAccountJson)
                    : GoogleCredential.FromFile(serviceAccountPath!);

                if (credential.IsCreateScopedRequired)
                {
                    credential = credential.CreateScoped(scopes);
                }

                var serviceAccountUser = _configuration["Google:ServiceAccountUser"];
                if (!string.IsNullOrWhiteSpace(serviceAccountUser))
                {
                    credential = credential.CreateWithUser(serviceAccountUser);
                }
            }
            else
            {
                var enableInteractive = _configuration.GetValue<bool?>("Google:EnableInteractiveAuth") ?? false;
                if (!enableInteractive)
                {
                    throw new InvalidOperationException(
                        "Autenticacao interativa desabilitada. Configure Google:ServiceAccountJson ou Google:ServiceAccountPath " +
                        "(e opcionalmente Google:ServiceAccountUser) para usar em servidor."
                    );
                }

                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = _configuration["Google:ClientId"],
                        ClientSecret = _configuration["Google:ClientSecret"]
                    },
                    scopes,
                    "user",
                    CancellationToken.None
                );
            }

            _service = new Google.Apis.Classroom.v1.ClassroomService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "StudyHelper"
            });

            return _service;
        }

        public async Task<List<ClassroomCourse>> GetCoursesAsync()
        {
            var service = await GetServiceAsync();
            var request = service.Courses.List();
            request.CourseStates = CoursesResource.ListRequest.CourseStatesEnum.ACTIVE;

            var response = await request.ExecuteAsync();

            return response.Courses?.Select(c => new ClassroomCourse
            {
                Id = c.Id,
                Name = c.Name,
                Section = c.Section,
                EnrollmentCode = c.EnrollmentCode
            }).ToList() ?? new List<ClassroomCourse>();
        }

        public async Task<List<ClassroomMaterial>> GetMaterialsAsync(string courseId)
        {
            var service = await GetServiceAsync();

            if (string.IsNullOrEmpty(courseId))
                return new List<ClassroomMaterial>();

            var request = service.Courses.CourseWorkMaterials.List(courseId);
            var response = await request.ExecuteAsync();

            return response.CourseWorkMaterial?.Select(m => new ClassroomMaterial
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                CourseId = courseId,
                CreatedAt = m.CreationTime is DateTime dt ? dt : DateTime.Now,
                AttachmentUrls = m.Materials?
                    .Where(a => a.DriveFile?.DriveFile?.AlternateLink != null)
                    .Select(a => a.DriveFile.DriveFile.AlternateLink)
                    .ToList() ?? new List<string>()
            }).ToList() ?? new List<ClassroomMaterial>();
        }

        public async Task<List<ClassroomAssignment>> GetAssignmentsAsync(string courseId)
        {
            var service = await GetServiceAsync();
            var request = service.Courses.CourseWork.List(courseId);
            var response = await request.ExecuteAsync();

            return response.CourseWork?.Select(a => new ClassroomAssignment
            {
                Id = a.Id,
                Title = a.Title,
                Description = a.Description,
                CourseId = courseId,
                MaxPoints = (int)(a.MaxPoints ?? 0),
                State = a.State,
                DueDate = a.DueDate != null
                    ? new DateTime(a.DueDate.Year ?? 2024, a.DueDate.Month ?? 1, a.DueDate.Day ?? 1)
                    : null
            }).ToList() ?? new List<ClassroomAssignment>();
        }

        public async Task<List<ClassroomAssignment>> GetPendingAssignmentsAsync()
        {
            var courses = await GetCoursesAsync();
            var allAssignments = new List<ClassroomAssignment>();

            foreach (var course in courses)
            {
                var assignments = await GetAssignmentsAsync(course.Id);
                var pending = assignments.Where(a =>
                    a.DueDate.HasValue &&
                    a.DueDate.Value >= DateTime.Now &&
                    !a.IsCompleted
                ).ToList();

                allAssignments.AddRange(pending);
            }

            return allAssignments.OrderBy(a => a.DueDate).ToList();
        }
    }
    
