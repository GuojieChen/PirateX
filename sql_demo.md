
```
var list = CacheableDao.Select<Activity>(0, e => e.OpenAt < dtNow && e.CloseAt > dtNow);
list.FirstOrDefault(item => item.Type == type && item.Days.Contains(a));
```

```

```
