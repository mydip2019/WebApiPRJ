#region using namespaces.
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using BusinessEntities;
using DataModel;
using DataModel.GenericRepository;
using DataModel.UnitOfWork;
using Moq;
using NUnit.Framework;
using TestsHelper;

#endregion

namespace BusinessServices.Tests
{
    /// <summary>
    /// Project Service Test
    /// </summary>
    public class ProjectServicesTest
    {
        #region Variables

        private IProjectServices _projectServices;
        private IUnitOfWork _unitOfWork;
        private List<Project> _projects;
        public List<Contact> _contacts;
        private GenericRepository<Project> _projectRepository;
        private WebApiDbEntities _dbEntities;
        private GenericRepository<Contact> _contactRepository;
        #endregion

        #region Test fixture setup

        /// <summary>
        /// Initial setup for tests
        /// </summary>
        [TestFixtureSetUp]
        public void Setup()
        {
            _projects = SetUpProjects();
            _contacts = SetUpContacts();
        }

        #endregion

        #region Setup

        /// <summary>
        /// Re-initializes test.
        /// </summary>
        [SetUp]
        public void ReInitializeTest()
        {
            _projects = SetUpProjects();
            _contacts = SetUpContacts();
            _dbEntities = new Mock<WebApiDbEntities>().Object;
            _projectRepository = SetUpProjectRepository();
            _contactRepository = SetUpContactRepository();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.SetupGet(s => s.ContactRepository).Returns(_contactRepository);
            unitOfWork.SetupGet(s => s.ProjectRepository).Returns(_projectRepository);          
            _unitOfWork = unitOfWork.Object;
            _projectServices = new ProjectServices(_unitOfWork);

        }

        #endregion

        #region Private member methods

        /// <summary>
        /// Setup dummy contacts data
        /// </summary>
        /// <returns></returns>
        private static List<Contact> SetUpContacts()
        {
            var prodId = new int();
            var contacts = DataInitializer.GetAllContact();
            foreach (Contact prod in contacts)
                prod.Id = ++prodId;
            return contacts;

        }

        /// <summary>
        /// Setup dummy repository
        /// </summary>
        /// <returns></returns>
        private GenericRepository<Project> SetUpProjectRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<Project>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior
            mockRepo.Setup(p => p.GetAll()).Returns(_projects);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, Project>(
                             id => _projects.Find(p => p.Id.Equals(id))));

            mockRepo.Setup(p => p.Insert((It.IsAny<Project>())))
                .Callback(new Action<Project>(newProject =>
                {
                    dynamic maxProjectID = _projects.Last().Id;
                    dynamic nextProjectID = maxProjectID + 1;
                    newProject.Id = nextProjectID;
                    _projects.Add(newProject);
                }));

            mockRepo.Setup(p => p.Update(It.IsAny<Project>()))
                .Callback(new Action<Project>(prod =>
                {
                    var oldProject = _projects.Find(a => a.Id == prod.Id);
                    oldProject = prod;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Project>()))
                .Callback(new Action<Project>(prod =>
                {
                    var projectToRemove =
                        _projects.Find(a => a.Id == prod.Id);

                    if (projectToRemove != null)
                        _projects.Remove(projectToRemove);
                }));

            // Return mock implementation object
            return mockRepo.Object;
        }


        /// <summary>
        /// Setup dummy repository
        /// </summary>
        /// <returns></returns>
        private GenericRepository<Contact> SetUpContactRepository()
        {
            // Initialise repository
            var mockRepo = new Mock<GenericRepository<Contact>>(MockBehavior.Default, _dbEntities);

            // Setup mocking behavior
            mockRepo.Setup(p => p.GetAll()).Returns(_contacts);

            mockRepo.Setup(p => p.GetByID(It.IsAny<int>()))
                .Returns(new Func<int, Contact>(
                             id => _contacts.Find(p => p.Id.Equals(id))));

            mockRepo.Setup(p => p.Insert((It.IsAny<Contact>())))
                .Callback(new Action<Contact>(newContact =>
                {
                    dynamic maxContactID = _contacts.Last().Id;
                    dynamic nextContactID = maxContactID + 1;
                    newContact.Id = nextContactID;
                    _contacts.Add(newContact);
                }));

            mockRepo.Setup(p => p.Update(It.IsAny<Contact>()))
                .Callback(new Action<Contact>(prod =>
                {
                    var oldContact = _contacts.Find(a => a.Id == prod.Id);
                    oldContact = prod;
                }));

            mockRepo.Setup(p => p.Delete(It.IsAny<Contact>()))
                .Callback(new Action<Contact>(prod =>
                {
                    var contactToRemove =
                        _contacts.Find(a => a.Id == prod.Id);

                    if (contactToRemove != null)
                        _contacts.Remove(contactToRemove);
                }));

            // Return mock implementation object
            return mockRepo.Object;
        }


        /// <summary>
        /// Setup dummy projects data
        /// </summary>
        /// <returns></returns>
        private static List<Project> SetUpProjects()
        {
            var prodId = new int();
            var projects = DataInitializer.GetAllProject();
            foreach (Project prod in projects)
                prod.Id = ++prodId;
            return projects;

        }

        #endregion

        #region Unit Tests

        /// <summary>
        /// Service should return all the projects
        /// </summary>
        [Test]
        public void GetAllProjectsTest()
        {
            var projects = _projectServices.GetAllProject();
            if (projects != null)
            {
                var projectList =
                    projects.Select(
                        projectEntity =>
                        new Project
                        {
                            Id = projectEntity.id,
                            ProjectName = projectEntity.projectName,
                            ContactId = projectEntity.contactId,
                            StartDate = projectEntity.startDate,
                            EndDate = projectEntity.endDate,
                            Priority = projectEntity.priority,
                            IsSetDate = projectEntity.isSetDate,
                        }).
                        ToList();
                var comparer = new ProjectComparer();
                CollectionAssert.AreEqual(
                    projectList.OrderBy(project => project, comparer),
                    _projects.OrderBy(project => project, comparer), comparer);
            }
        }

        /// <summary>
        /// Service should return null
        /// </summary>
        [Test]
        public void GetAllProjectsTestForNull()
        {
            _projects.Clear();
            var projects = _projectServices.GetAllProject();
            Assert.Null(projects);
            SetUpProjects();
        }

        /// <summary>
        /// Service should return project if correct id is supplied
        /// </summary>
        [Test]
        public void GetProjectByRightIdTest()
        {
            var mroject = _projectServices.GetProjectById(2);
            if (mroject != null)
            {
                Mapper.CreateMap<ProjectEntity, Project>();
                var projectModel = Mapper.Map<ProjectEntity, Project>(mroject);
                AssertObjects.PropertyValuesAreEquals(projectModel, _projects.Find(a => a.ProjectName.Contains("Test2")));
            }
        }

        /// <summary>
        /// Service should return null
        /// </summary>
        [Test]
        public void GetProjectByWrongIdTest()
        {
            var project = _projectServices.GetProjectById(0);
            Assert.Null(project);
        }

        /// <summary>
        /// Add new project test
        /// </summary>
        [Test]
        public void AddNewProjectTest()
        {
            var newProject = new ProjectEntity()
            {
                projectName = "Test 3 Project",
                isSetDate = false,
                startDate = null,
                endDate = null,
                contactId = null,
                priority = 4
            };

            var maxProjectIDBeforeAdd = _projects.Max(a => a.Id);
            newProject.id = maxProjectIDBeforeAdd + 1;
            _projectServices.CreateProject(newProject);
            var addedproject = new Project() { ProjectName = newProject.projectName, Id = newProject.id, StartDate = newProject.startDate, EndDate = newProject.endDate, Priority = newProject.priority, ContactId = newProject.contactId, IsSetDate = newProject.isSetDate };
            AssertObjects.PropertyValuesAreEquals(addedproject, _projects.Last());
            Assert.That(maxProjectIDBeforeAdd + 1, Is.EqualTo(_projects.Last().Id));
        }

        /// <summary>
        /// Update project test
        /// </summary>
        [Test]
        public void UpdateProjectLastNameTest()
        {
            var firstProject = _projects.First();
            firstProject.ProjectName = "Testing Project updated";
            var updatedProject = new ProjectEntity()
            { projectName = firstProject.ProjectName, id = firstProject.Id };
            _projectServices.UpdateProject(firstProject.Id, updatedProject);
            Assert.That(firstProject.Id, Is.EqualTo(1)); // hasn't changed
            Assert.That(firstProject.ProjectName, Is.EqualTo("Testing Project updated")); // Project name changed
        }

        /// <summary>
        /// Update project test
        /// </summary>
        [Test]
        public void UpdateProjectTest()
        {
            var firstProject = _projects.First();
            firstProject.ProjectName = "TestingProject updated";
            firstProject.Priority = 5;
            firstProject.ContactId = 3;
            firstProject.EndDate = DateTime.Now.AddMonths(3);
            
            var updatedProject = new ProjectEntity()
            { projectName = firstProject.ProjectName, id = firstProject.Id, priority = firstProject.Priority, contactId = firstProject.ContactId , endDate = firstProject.EndDate};
            _projectServices.UpdateProject(firstProject.Id, updatedProject);
            Assert.That(firstProject.Id, Is.EqualTo(1)); // hasn't changed
            Assert.That(firstProject.ProjectName, Is.EqualTo("TestingProject updated"));  
            Assert.That(firstProject.Priority, Is.EqualTo(5));  
            Assert.That(firstProject.ContactId, Is.EqualTo(3));
            Assert.That(firstProject.EndDate, Is.EqualTo(DateTime.Now.AddMonths(3)));
        }


        /// <summary>
        /// Delete project test
        /// </summary>
        [Test]
        public void DeleteProjectTest()
        {
            int startMaxID = _projectServices.GetAllProject().Count();
            // _projects.Max(a => a.Id); // Before removal
            var lastProject = _projects.Last();

            // Remove last Project
            _projectServices.DeleteProject(lastProject.Id);
            int endMaxID = _projectServices.GetAllProject().Count();
            //Assert.That(maxID, Is.GreaterThan(_projects.Max(a => a.Id))); // Max id reduced by 1
            Assert.IsTrue(endMaxID == (startMaxID - 1));
        }

        #endregion


        #region Tear Down

        /// <summary>
        /// Tears down each test data
        /// </summary>
        [TearDown]
        public void DisposeTest()
        {
            _projectServices = null;
            _unitOfWork = null;
            _projectRepository = null;
            if (_dbEntities != null)
                _dbEntities.Dispose();
            _projects = null;
        }

        #endregion

        #region TestFixture TearDown.

        /// <summary>
        /// TestFixture teardown
        /// </summary>
        [TestFixtureTearDown]
        public void DisposeAllObjects()
        {
            _projects = null;
        }

        #endregion
    }
}
