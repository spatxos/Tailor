Native/Default是标准版网站类库

Tailor.Custom* 是定制化网站，可以在此路径下继承Native/Default的对应Controller，利用overvide对需要重写的action进行重写，不需要重写的不进行overvide即可，或者对cshtml进行重写，不需要重写的不在对应路径下增加cshtml文件即可

Api.Versioning存在bug:https://github.com/microsoft/aspnet-api-versioning/issues/630 暂时可使用不同的Controller名字来规避

似乎Api.Versioning对于可用版本的控制只是根据Controller的名字来的，当我在User目录下添加了一个v3的HomeComtroller时，我的http://localhost:port/home/getjson 就无法访问了， 返回的api-supported-versions是1.0,2.0,3.0了。。。。

bug已修复，但是使用注入的办法无法对area不同的同名controller进行修改，除非将AddApiVersioning方法和ApiVersionCollator重写或者将Api.Versioning源码拉下来，修改之后不引用nuget中的类库，我现在使用的是源码拉下来修改引用的办法
