# Swashbuckle.Extensions
扩展了一些swagger 的功能
1.忽略参数
c.IgnoreParamters(p =>
{

});
1.自定义api路径
c.ResolverRelativePath(path =>
{

});
3.忽略controller 加 [Swashbuckle.Swagger.Annotations.SwaggerIgnore] 特性
c.IgnoreControllers();

4.接口标签支持注释说明，契约文档时原来的throw,改成全名。 
