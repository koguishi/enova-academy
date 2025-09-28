namespace enova_academy.Domain;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class CourseCapacityExceededException : DomainException
{
    public CourseCapacityExceededException() 
        : base("Course capacity exceeded.") { }
}

public class EnrollmentAlreadyTakenException : DomainException
{
    public EnrollmentAlreadyTakenException() 
        : base("Enrollment already taken for this course.") { }
}
