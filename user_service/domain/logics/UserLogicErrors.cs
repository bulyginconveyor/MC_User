using user_service.services.result.errors.@base;

namespace user_service.domain.logics;

public class UserLogicErrors
{
    public Error UserNameEmpty
        => new("userLogic.UserNameEmpty", "Имя пользователя не может быть пустым");
    public Error EmailEmpty
        => new("userLogic.EmailEmpty", "Email не может быть пустым");
    public Error PasswordEmpty
        => new("userLogic.PasswordEmpty", "Пароль не может быть пустым");

    public Error PasswordNotValid
        => new("userLogic.PasswordEmpty", "Недостаточно хороший пароль! \n " +
                                                            "Пароль должен: \n" +
                                                            " - Иметь длину не менее 8 символов \n" +
                                                            " - Содержать в себе маленькие и большие латинские буквы \n" +
                                                            " - Содержать в себе цифры \n" +
                                                            " - Содержать в себе специальные символы (@%&\\^$!#*_-)");
    
    public Error EmailNotConfirmed 
        => new("userLogic.EmailNotConfirmed", "Email не подтвержден");

    public Error NoAuthorize
        => new("userLogic.NoAuthorize", "Неверный логин или пароль");

    public Error UserNameExists
        => new("userLogic.UserNameExists", "Пользователь с таким именем уже существует");

    public Error EmailExists
        => new("userLogic.EmailExists", "Пользователь с таким email уже существует");

    public Error IsAttack 
        => new ("userLogic.IsAttack", "Слишком много попыток авторизации, попробуйте позже");

    public Error UserNotFound
        => new("userLogic.UserNotFound", "Такого пользователя не существует");

    public Error TokenInvalid
        => new("userLogic.TokenInvalid", "Токен не действителен");
}
