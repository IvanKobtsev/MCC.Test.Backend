```

BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3775)
AMD Ryzen 5 5600H with Radeon Graphics, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.203
  [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2
  DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX2


```
| Method                               | Mean      | Error     | StdDev    | Rank | Gen0      | Gen1      | Gen2     | Allocated |
|------------------------------------- |----------:|----------:|----------:|-----:|----------:|----------:|---------:|----------:|
| GetAllUsersAsync_100_NoFilters       |  6.347 ms | 0.0724 ms | 0.0642 ms |    1 |  125.0000 |   46.8750 |        - |   1.01 MB |
| GetAllUsersAsync_100_DoubleFiltered  |  6.966 ms | 0.1221 ms | 0.1499 ms |    2 |  125.0000 |   62.5000 |        - |   1.07 MB |
| GetAllUsersAsync_100_Filtered        |  7.010 ms | 0.1359 ms | 0.1861 ms |    2 |  125.0000 |   46.8750 |        - |   1.04 MB |
| GetAllUsersAsync_500_NoFilters       | 20.647 ms | 0.1541 ms | 0.1203 ms |    3 |  625.0000 |  593.7500 | 312.5000 |   5.53 MB |
| GetAllUsersAsync_500_Filtered        | 20.726 ms | 0.2019 ms | 0.1686 ms |    3 |  625.0000 |  593.7500 | 312.5000 |   5.42 MB |
| GetAllUsersAsync_500_DoubleFiltered  | 21.144 ms | 0.1699 ms | 0.1419 ms |    3 |  625.0000 |  593.7500 | 312.5000 |   5.47 MB |
| GetAllUsersAsync_1000_DoubleFiltered | 27.720 ms | 0.5240 ms | 0.5382 ms |    4 |  968.7500 |  875.0000 | 281.2500 |   6.91 MB |
| GetAllUsersAsync_1000_Filtered       | 38.936 ms | 0.7775 ms | 1.1151 ms |    5 | 1461.5385 | 1230.7692 | 538.4615 |  10.82 MB |
| GetAllUsersAsync_1000_NoFilters      | 39.059 ms | 0.7754 ms | 1.9596 ms |    5 | 1538.4615 | 1230.7692 | 538.4615 |  10.92 MB |
