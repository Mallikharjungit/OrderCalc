cmake_minimum_required(VERSION 3.15)
project(OrderCalc CXX)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

add_library(ordercalc_lib
    src/Discounts.cpp
    src/Tax.cpp
    src/Shipping.cpp
    src/Inventory.cpp
    src/OrderCalculator.cpp
)
target_include_directories(ordercalc_lib PUBLIC include)

add_executable(ordercalc src/main.cpp)
target_link_libraries(ordercalc PRIVATE ordercalc_lib)
