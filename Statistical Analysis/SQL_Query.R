library(DBI)
library(dplyr)
library(dbplyr)
library(odbc)
library(sqldf)

library(tidymodels)
library(tidyverse)   # for reading in data, graphing, and cleaning
library(mdsr)        # for accessing some databases - goes with Modern Data Science with R textbook
library(RMySQL)      # for accessing MySQL databases
library(RSQLite)     # for accessing SQLite databases

library(gghighlight)
library(dbplot)

con <- dbConnect(odbc(), 
                 Driver="SQL Server",
                 Server="SERVER",
                 Database ="train_db", 
                 Uid="prt", 
                 Pwd="smrmprt", 
                 Port=3306)

query1<-tbl(con,sql("Select * FROM car where DateTime >= '02/01/2022 01:17:53' AND DateTime <= '02/01/2022 01:39:02'"))
query2<-tbl(con,sql("Select * FROM detector"))
print(query1)

# Create Plots
query1 %>%
  ggplot(aes(x = MaxLat,
             y = fct_reorder(Owner, MaxLat, median))) +
  geom_col(fill = "lightblue") +
  scale_x_continuous(expand = c(0,0),
                     labels = scales::percent) +
  labs(x = NULL,
       y = NULL,
       title = "does this code work?") +
  theme_minimal()

query1 %>% ggplot(aes(x=MaxLat, y=SiteIndex)) + geom_point()
query1 %>% dbplot_histogram(MaxLat, binwidth = 1)

# query1 %>% remote_query() %>% build_sql(con = con)
query <- collect(query1)

# Create Linear Models
linear_reg() %>% set_engine("lm") 
linear_reg() %>% set_engine("lm") %>% fit(MaxVert ~ CarWeight, data = query)

#Disconnect from DataBase
#dbDisconnect(con)

