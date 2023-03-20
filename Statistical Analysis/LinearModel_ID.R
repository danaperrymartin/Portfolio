library(yaml)
library(tidypredict)
library(factoextra)
library(mclust)
library(ggplot2)
library(GGally)
library(data.table)

ggplotRegression <- function (fit) 
{
  
  require(ggplot2)
  
  ggplot(fit$model, aes_string(x = names(fit$model)[2], y = names(fit$model)[1])) + 
    geom_point() +
    stat_smooth(method = "lm", col = "red") +
    labs(title = paste("Adj R2 = ",signif(summary(fit)$adj.r.squared, 5),
                       "Intercept =",signif(fit$coef[[1]],5 ),
                       " Slope =",signif(fit$coef[[2]], 5),
                       " P =",signif(summary(fit)$coef[2,4], 5)))
}


buildlinearmodel <- function(trninfo,numberaxles,carinfo,cribinfo,cribaxlecount,filename) {
  maxlat        <- c()
  maxvert       <- c()
  magnitude     <- c()
  directionl    <- c()
  directionv    <- c()
  peaknumber    <- c()
  carnumber     <- c()
  wheelinfo_tmp <- c()
  wheelinfo     <- NULL
  
  # Create a for statement to populate the list
  icount   = 1
  peakcounter = 1
  # print(length(trninfo))
  
  for (i in 1:length(trninfo)) 
  {
    peaknumber[i] <- as.integer(trninfo[[i]][1])
    maxvert[i]    <- as.double(trninfo[[i]][4])
    maxlat[i]     <- as.double(trninfo[[i]][5])
    magnitude[i]  <- sqrt(as.double(trninfo[[i]][4])^2+as.double(trninfo[[i]][5])^2)
    directionl[i] <- asin(maxlat[i]/magnitude[i])
    directionv[i] <- acos(maxvert[i]/magnitude[i])
  }
  
  carowner <-c()
  carnumber<-c()
  cartype  <-c()
  caraxles <-c()
  siteinfo <-c()
  peakcount_check<-c()
  icar    = 1
  icrib   = 1
  totalaxles = as.integer(carinfo[[icar]][6])
  for(i in 1:length(peaknumber))
  {
    if(i<length(trninfo))
    {
      carowner[i]   <- carinfo[[icar]][3]
      carnumber[i]  <- as.integer(carinfo[[icar]][4])
      cartype[i]    <- carinfo[[icar]][5]
      caraxles[i]   <- as.integer(carinfo[[icar]][6])
      siteinfo[i]   <- paste(cribinfo[[icrib]][5],cribinfo[[icrib]][6],cribinfo[[icrib]][7],cribinfo[[icrib]][8], sep=" ")
      
      if (as.integer(cribaxlecount[[icrib]][4])>numberaxles+20 || as.integer(cribaxlecount[[icrib]][4])<numberaxles-20)
      {
        peakcount_check[i]<- "Invalid" 
      }
      else
      {
        peakcount_check[i]<- "Valid" 
      }
      
      if(peaknumber[i]==totalaxles && icar<=length(carinfo))
      {
        if(peakcount_check[i]=="Valid")
        {
          
          if(icar==length(carinfo))
          {
            icar=1
            totalaxles = as.integer(carinfo[[icar]][6])
          }
          else if(icar<length(carinfo))
          {
            icar=icar+1
            totalaxles = totalaxles+as.integer(carinfo[[icar]][6])
          }
          else if(icar>length(carinfo))
          {
            icar=icar-1
            totalaxles = totalaxles+as.integer(carinfo[[icar]][6])
          }
        }
        else if(peakcount_check[i]=="Invalid")
        {
          if(icar<length(carinfo))
          {
            icar = icar+1
            totalaxles = totalaxles+as.integer(carinfo[[icar]][6])
          }
          else
          {
            totalaxles = peaknumber[i]
            icar = icar
          }
        }
      }
      else 
      {
        if(peaknumber[i+1]>peaknumber[i])
        {
          icar = icar
        }
        else if(peaknumber[i+1]<peaknumber[i])
        {
          icar=1
          totalaxles=as.integer(carinfo[[icar]][6])
        }
      }
      

      if(as.integer(trninfo[[i+1]][1])>peaknumber[i] && peaknumber[i]<=numberaxles)
      {
        wheelinfo_tmp[peaknumber[i]] = maxlat[i]
        peakcounter = peakcounter+1
      }
      else if(as.integer(trninfo[[i+1]][1])<peaknumber[i] && peaknumber[i]<=numberaxles)
      {
        wheelinfo_tmp[peaknumber[i]] = maxlat[i]
        # wheelinfo<-cbind(wheelinfo,wheelinfo_tmp)
        wheelinfo_tmp <- c()
        peakcounter=1
      }
      if(peaknumber[i]==as.integer(cribaxlecount[[icrib]][4])||as.integer(cribaxlecount[[icrib]][4])==0)
      {
        icrib=icrib+1
      }
    }
    else
    {
      carowner[i]   <- carinfo[[icar]][3]
      carnumber[i]  <- as.integer(carinfo[[icar]][4])
      cartype[i]    <- carinfo[[icar]][5]
      caraxles[i]   <- as.integer(carinfo[[icar]][6])
      siteinfo[i]   <- paste(cribinfo[[icrib]][5],cribinfo[[icrib]][6],cribinfo[[icrib]][7],cribinfo[[icrib]][8], sep=" ")
      
      if (as.integer(cribaxlecount[[icrib]][4])>numberaxles+20 || as.integer(cribaxlecount[[icrib]][4])<numberaxles-20)
      {
        peakcount_check[i]<- "Invalid" 
      }
      else
      {
        peakcount_check[i]<- "Valid" 
      }
    }
  
  }
  
  mtx<-matrix(c(maxlat,directionl),nrow=length(directionl))
  df_mtx<-as.data.frame(mtx)
  
  #kmeans clustering
  # km.res <- kmeans(directionl, 3, iter.max = 1000, nstart = 30)
  # colnames(df_mtx) <- c("x", "y")
  # fviz_cluster(km.res, df_mtx ,nrow=length(mtx), ellipse.type = "norm")
  
  #Density based clustering
  # db = dbscan(df_mtx, 0.1, 3)
  # hullplot(df_mtx, db$cluster)
  
  #Model based clustering
  mb3 = Mclust(df_mtx, 3)
  # plot(mb3, what=c("classification"))
  clusterresults = data.frame(df_mtx,cluster=mb3$classification)
  
  clusterresults[clusterresults[,3]==1,1]->maxlat_a
  carowner[clusterresults[,3]==1]->carowner_a
  carnumber[clusterresults[,3]==1]->carnumber_a
  
  clusterresults[clusterresults[,3]==2,1]->maxlat_b
  carowner[clusterresults[,3]==2]->carowner_b
  carnumber[clusterresults[,3]==2]->carnumber_b
  
  clusterresults[clusterresults[,3]==3,1]->maxlat_c
  carowner[clusterresults[,3]==2]->carowner_c
  carnumber[clusterresults[,3]==2]->carnumber_c
  
  clusterresults[clusterresults[,3]==1,2]->directionl_a
  clusterresults[clusterresults[,3]==2,2]->directionl_b
  clusterresults[clusterresults[,3]==3,2]->directionl_c
  
  if(file.exists("C:/CodeProjects/Statistical Analysis/HuntingModel_a.rds"))
  {
    filename <- file("C:/CodeProjects/Statistical Analysis/HuntingModel_a.rds")
    model_a <- readRDS(filename)
    linearmodel_a<-update(model_a, . ~ . - 1, data=data.frame(maxlat_a,directionl_a))
    pa=predict(linearmodel_a,interval="confidence",level=0.95)
    directionl_a[directionl_a<(pa[,2]*1.5)|directionl_a>(pa[,3]*1.5)]=1
    saveRDS(linearmodel_a, filename)
    close(filename)
    
    filename <- file("C:/CodeProjects/Statistical Analysis/HuntingModel_b.rds")
    model_b <- readRDS(filename)
    linearmodel_b<-update(model_b, . ~ . - 1, data=data.frame(maxlat_b,directionl_b))
    pb=predict(linearmodel_b,interval="confidence",level=0.95)
    directionl_b[directionl_b<(pb[,2]*1.5)|directionl_b>(pb[,3]*1.5)]=1
    saveRDS(linearmodel_b, filename)
    close(filename)
    
    filename <- file("C:/CodeProjects/Statistical Analysis/HuntingModel_c.rds")
    model_c <- readRDS(filename)
    linearmodel_c<-update(model_c, . ~ . - 1, data=data.frame(maxlat_c,directionl_c))
    pc=predict(linearmodel_c,interval="confidence",level=0.95)
    directionl_c[directionl_c<(pc[,2]*1.5)|directionl_c>(pc[,3]*1.5)]=1
    saveRDS(linearmodel_c, filename)
    close(filename)
    
    unique(cbind(carowner_a[directionl_a==1],carnumber_a[directionl_a==1]))->huntingflag_a
    unique(cbind(carowner_b[directionl_b==1],carnumber_b[directionl_b==1]))->huntingflag_b
    unique(cbind(carowner_c[directionl_c==1],carnumber_c[directionl_c==1]))->huntingflag_c
    unique(rbind(huntingflag_a,huntingflag_b,huntingflag_c))->huntingflag
    
    
    # if(file.exists("C:/CodeProjects/Statistical Analysis/Hunters.hnt"))
    # {
    #   filecon<-file("C:/CodeProjects/Statistical Analysis/Hunters.hnt","r")
    #   read.table(filecon,sep="\t")->tracked_hunters
    #   close(filecon)
    #   faultnumber<-as.data.frame(table(rbind(tracked_hunters,huntingflag)))
    #   faultnumber[order(faultnumber[,3],decreasing=FALSE),]
    #   # faultnumber[,3]<-sum(faultnumber[,3],faultnumber[,4])
    #   # faultnumber<-matrix(c(faultnumber[,1],faultnumber[,2],faultnumber[,3]),nrow=length(faultnumber[,2]))
    #   filecon<-file("C:/CodeProjects/Statistical Analysis/Hunters.hnt","w")
    #   # write.table(faultnumber,filecon,append=TRUE,sep="\t",row.names=FALSE,col.names = FALSE)
    #   dump(faultnumber,file="C:/CodeProjects/Statistical Analysis/Hunters.hnt")
    # }
    # else
    # {
    #   # filecon<-file("C:/CodeProjects/Statistical Analysis/Hunters.hnt","w")
    #   # write.table(huntingflag,filecon,append=TRUE,sep="\t",row.names=FALSE,col.names = FALSE)
    #   dump(huntingflag,"C:/CodeProjects/Statistical Analysis/Hunters.hnt")
    # }
    # 
    # fit1<-lm(directionl_c ~ maxlat_c,data=df_mtx)
    
    # fit1<-lm(directionl_a ~ maxlat_a,data=as.data.frame(matrix(c(maxlat_a,directionl_a),nrow=length(directionl_a))))
    # fit2<-lm(directionl_b ~ maxlat_b,data=as.data.frame(matrix(c(maxlat_b,directionl_b),nrow=length(directionl_b))))
    # fit3<-lm(directionl_c ~ maxlat_c,data=as.data.frame(matrix(c(maxlat_c,directionl_c),nrow=length(directionl_c))))
    # ggp <- ggplotRegression(fit1)+ggplotRegression(fit2)+ggplotRegression(fit3)
    
    # points(maxlat_a,directionl_a)
    # abline(model_a)
    # points(maxlat_b,directionl_b)
    # abline(model_b)
    # points(maxlat_c,directionl_c)
    # abline(model_c)
  }
  else
  {
    model_a <- lm(directionl_a ~ maxlat_a)
    filename = file("C:/CodeProjects/Statistical Analysis/HuntingModel_a.rds")
    saveRDS(linearmodel_a, filename)
    close(filename)
    
    model_b <- lm(directionl_b ~ maxlat_b)
    filename = file("C:/CodeProjects/Statistical Analysis/HuntingModel_b.rds")
    saveRDS(linearmodel_b, filename)
    close(filename)
    
    model_c <- lm(directionl_c ~ maxlat_c)
    filename = file("C:/CodeProjects/Statistical Analysis/HuntingModel_c.rds")
    saveRDS(linearmodel_c, filename)
    close
    
    # plot(ggplotRegression(lm(maxlat_a ~ directionl_a)))
    
    # points(maxlat_a,directionl_a)
    # abline(model_a)
    # points(maxlat_b,directionl_b)
    # abline(model_b)
    # points(maxlat_c,directionl_c)
    # abline(model_c)
  }
}

makeplotImbalance <- function (NearFarRatio_all_Track1, NearFarRatio_loco_Track1,NearFarRatio_all_Track2, NearFarRatio_loco_Track2) 
{
  
  # jpeg(file="C:/CodeProjects/Statistical Analysis/plot1.jpeg")
  x1 <- na.omit(NearFarRatio_loco_Track1)
  y1 <- na.omit(NearFarRatio_all_Track1)
  
  x2 <- na.omit(NearFarRatio_loco_Track2)
  y2 <- na.omit(NearFarRatio_all_Track2)
  # 
  # jpeg(file = "C://CodeProjects//Statistical Analysis//plot.jpeg")
  
  points(x1,y1,col="blue",pch = 19)
  points(x2,y2,col="red",pch = 17)
  
  # sample_data <- data.frame(x1, y1, x2, y2)
  # ggpairs(sample_data)
  # dev.off()
}

ImbalanceAnalysis <- function(trninfo,numberaxles,carinfo,cribinfo,cribaxlecount,filename) {
  maxlat        <- c()
  maxvert       <- c()
  magnitude     <- c()
  directionl    <- c()
  directionv    <- c()
  peaknumber    <- c()
  carnumber     <- c()
  wheelinfo_tmp <- c()
  wheelinfo     <- NULL
  
  # Create a for statement to populate the list
  icount   = 1
  peakcounter = 1
  # print(length(trninfo))
  
  for (i in 1:length(trninfo)) 
  {
    peaknumber[i] <- as.integer(trninfo[[i]][1])
    maxvert[i]    <- as.double(trninfo[[i]][4])
    maxlat[i]     <- as.double(trninfo[[i]][5])
    magnitude[i]  <- sqrt(as.double(trninfo[[i]][4])^2+as.double(trninfo[[i]][5])^2)
    directionl[i] <- asin(maxlat[i]/magnitude[i])
    directionv[i] <- acos(maxvert[i]/magnitude[i])
  }
  
  carowner <-c()
  carnumber<-c()
  cartype  <-c()
  caraxles <-c()
  siteinfo <-c()
  peakcount_check<-c()
  icar    = 1
  icrib   = 1
  totalaxles = as.integer(carinfo[[icar]][6])
  for(i in 1:length(peaknumber))
  {
    if(i<length(trninfo))
    {
      carowner[i]   <- carinfo[[icar]][3]
      carnumber[i]  <- as.integer(carinfo[[icar]][4])
      cartype[i]    <- carinfo[[icar]][5]
      caraxles[i]   <- as.integer(carinfo[[icar]][6])
      siteinfo[i]   <- paste(cribinfo[[icrib]][5],cribinfo[[icrib]][6],cribinfo[[icrib]][7],cribinfo[[icrib]][8], sep=" ")
      
      if (as.integer(cribaxlecount[[icrib]][4])>numberaxles+20 || as.integer(cribaxlecount[[icrib]][4])<numberaxles-20)
      {
        peakcount_check[i]<- "Invalid" 
      }
      else
      {
        peakcount_check[i]<- "Valid" 
      }
      
      if(peaknumber[i]==totalaxles && icar<=length(carinfo))
      {
        if(peakcount_check[i]=="Valid")
        {
          
          if(icar==length(carinfo))
          {
            icar=1
            totalaxles = as.integer(carinfo[[icar]][6])
          }
          else if(icar<length(carinfo))
          {
            icar=icar+1
            totalaxles = totalaxles+as.integer(carinfo[[icar]][6])
          }
          else if(icar>length(carinfo))
          {
            icar=icar-1
            totalaxles = totalaxles+as.integer(carinfo[[icar]][6])
          }
        }
        else if(peakcount_check[i]=="Invalid")
        {
          if(icar<length(carinfo))
          {
            icar = icar+1
            totalaxles = totalaxles+as.integer(carinfo[[icar]][6])
          }
          else
          {
            totalaxles = peaknumber[i]
            icar = icar
          }
        }
      }
      else 
      {
        if(peaknumber[i+1]>peaknumber[i])
        {
          icar = icar
        }
        else if(peaknumber[i+1]<peaknumber[i])
        {
          icar=1
          totalaxles=as.integer(carinfo[[icar]][6])
        }
      }
      
      
      if(as.integer(trninfo[[i+1]][1])>peaknumber[i] && peaknumber[i]<=numberaxles)
      {
        wheelinfo_tmp[peaknumber[i]] = maxlat[i]
        peakcounter = peakcounter+1
      }
      else if(as.integer(trninfo[[i+1]][1])<peaknumber[i] && peaknumber[i]<=numberaxles)
      {
        wheelinfo_tmp[peaknumber[i]] = maxlat[i]
        # wheelinfo<-cbind(wheelinfo,wheelinfo_tmp)
        wheelinfo_tmp <- c()
        peakcounter=1
      }
      if(peaknumber[i]==as.integer(cribaxlecount[[icrib]][4])||as.integer(cribaxlecount[[icrib]][4])==0)
      {
        icrib=icrib+1
      }
    }
    else
    {
      carowner[i]   <- carinfo[[icar]][3]
      carnumber[i]  <- as.integer(carinfo[[icar]][4])
      cartype[i]    <- carinfo[[icar]][5]
      caraxles[i]   <- as.integer(carinfo[[icar]][6])
      siteinfo[i]   <- paste(cribinfo[[icrib]][5],cribinfo[[icrib]][6],cribinfo[[icrib]][7],cribinfo[[icrib]][8], sep=" ")
      
      if (as.integer(cribaxlecount[[icrib]][4])>numberaxles+20 || as.integer(cribaxlecount[[icrib]][4])<numberaxles-20)
      {
        peakcount_check[i]<- "Invalid" 
      }
      else
      {
        peakcount_check[i]<- "Valid" 
      }
    }
    
  }
  
  sum_mtx<-matrix(c(carowner, cartype, siteinfo, maxvert, maxlat, peakcount_check),nrow=length(maxvert))
  df_summtx<-as.data.frame(sum_mtx)
  colnames(df_summtx)<-c('carowner', 'cartype','siteinfo','maxvert', 'maxlat','peakcount_check')
  write.csv(df_summtx,paste("C:\\CodeProjects\\Statistical Analysis\\Summary_",strsplit(strsplit(filename,"/")[[1]][5],".trn")[[1]][1],".csv"), row.names = FALSE)
  
  tmp_df_Near <- df_summtx[grep("Near", df_summtx$siteinfo),]
  Near_df_all<- tmp_df_Near[grep("Valid", tmp_df_Near$peakcount_check),]
  Near_df_loco<- Near_df_all[grep("5", Near_df_all$cartype),]
  
  tmp_df_Far <- df_summtx[grep("Far", df_summtx$siteinfo),]
  Far_df_all<- tmp_df_Far[grep("Valid", tmp_df_Far$peakcount_check),]
  Far_df_loco<- Far_df_all[grep("5", Far_df_all$cartype),]
  
  Near_df_all_Track1 <-c()
  Near_df_loco_Track1<-c()
  Far_df_all_Track1  <-c()
  Far_df_loco_Track1 <-c()
  
  Near_df_all_Track2 <-c()
  Near_df_loco_Track2<-c()
  Far_df_all_Track2  <-c()
  Far_df_loco_Track2 <-c()
  
  if(length(grep("Track1", strsplit(strsplit(filename,"/")[[1]][length(strsplit(filename,"/")[[1]])],".trn")[[1]][1]))>0)
  {
    Near_df_all_Track1<- Near_df_all
    Near_df_loco_Track1<- Near_df_loco
    Far_df_all_Track1<- Far_df_all
    Far_df_loco_Track1<- Far_df_loco
  }
  else if(length(grep("Track2", strsplit(strsplit(filename,"/")[[1]][length(strsplit(filename,"/")[[1]])],".trn")[[1]][1]))>0)
  {
    Near_df_all_Track2<- Near_df_all
    Near_df_loco_Track2<- Near_df_loco
    Far_df_all_Track2<- Far_df_all
    Far_df_loco_Track2<- Far_df_loco
  }
  
  Near_avg_all<-mean(as.integer(Near_df_all$maxvert))
  Near_avg_loco<-mean(as.integer(Near_df_loco$maxvert))
  
  Near_avg_all_Track1<-mean(as.integer(Near_df_all_Track1$maxvert))
  Near_avg_loco_Track1<-mean(as.integer(Near_df_loco_Track1$maxvert))
  Near_avg_all_Track2<-mean(as.integer(Near_df_all_Track2$maxvert))
  Near_avg_loco_Track2<-mean(as.integer(Near_df_loco_Track2$maxvert))
  
  Far_avg_all<-mean(as.integer(Far_df_all$maxvert))
  Far_avg_loco<-mean(as.integer(Far_df_loco$maxvert))
  
  Far_avg_all_Track1<-mean(as.integer(Far_df_all_Track1$maxvert))
  Far_avg_loco_Track1<-mean(as.integer(Far_df_loco_Track1$maxvert))
  Far_avg_all_Track2<-mean(as.integer(Far_df_all_Track2$maxvert))
  Far_avg_loco_Track2<-mean(as.integer(Far_df_loco_Track2$maxvert))
  
  NearFarRatio_all         <- Near_avg_all/Far_avg_all
  NearFarRatio_all_Track1  <- Near_avg_all_Track1/Far_avg_all_Track1
  NearFarRatio_all_Track2  <- Near_avg_all_Track2/Far_avg_all_Track2
  NearFarRatio_loco        <- Near_avg_loco/Far_avg_loco
  NearFarRatio_loco_Track1 <- Near_avg_loco_Track1/Far_avg_loco_Track1
  NearFarRatio_loco_Track2 <- Near_avg_loco_Track2/Far_avg_loco_Track2
  
  makeplotImbalance(NearFarRatio_all_Track1, NearFarRatio_loco_Track1,NearFarRatio_all_Track2, NearFarRatio_loco_Track2)
  
  df_Imbalance<-as.data.frame(t(matrix(c(filename,Near_avg_all,Near_avg_loco,Far_avg_all,Far_avg_loco,NearFarRatio_all,NearFarRatio_loco))))
  colnames(df_Imbalance)<-c('FileName','Near_avg_all', 'Near_avg_loco','Far_avg_all','Far_avg_loco', 'NearFarRatio_all','NearFarRatio_loco')
  write.table(df_Imbalance,"C:\\CodeProjects\\Statistical Analysis\\Imbalance.csv", append = TRUE, quote = FALSE,row.names=FALSE,col.names=!file.exists("C:\\CodeProjects\\Statistical Analysis\\Imbalance.csv"),sep="    ")
}

parsefile <-function(x)
{
  if(any(grepl("Data", x, fixed = FALSE),na.rm = TRUE))
  {
    print(x) 
    readLines(x)->result
    return(result)
  }
}

splitline<-function(x)
{
  strsplit(grep("^[0-9]{1,3}\t+[0-9]{4}", x, value = TRUE),"\t") -> result
  return(result)
}

currdir = 'L:/LudlowCO/2022/09/05'
print(currdir)
savetop <- getwd()
setwd(currdir)
fls <- list.files(path=".",pattern=".trn$",full.name=TRUE,recursive=TRUE)
batchsize <- 10
fileprocessed <- 0

# par(mfrow=c(1,2))    # set the plotting area into a 1*2 array
plot.new()
plot.window(xlim=c(0,2),ylim=c(0,2))
axis(1)
axis(2)
box()
abline(a = 0, b = 1,col = "green")
# abline(h=0, v=0, lty="dashed")
title(main="LudlowCO Rail Imbalance Analysis")
title(xlab="NearFarRatio - Loco (-)")
#title(ylab="NearFarRatio - Train (-)")
# abline(v=seq(-2,2, by=1), col="gray")
# abline(h=seq(-20,20, by=10), col="gray")
grid(nx = 2, ny = 2, col = "lightgray", lty = "dotted",
     lwd = par("lwd"), equilogs = TRUE)
legend("topright", legend = c("Track 1","Track 2"),
       col =  c("blue", "red"),
       pch = c(19, 17) )

while(fileprocessed<length(fls))
{
  filtered_fls <-c()
  result       <-c()
  trnaxleinfo  <-c()
  carinfo      <-c()
  cribinfo     <-c()
  cribaxlecount<-c()
  
  Filter(function(x) any(grepl("/Data/", x)), fls)->filtered_fls
  sapply(na.omit(filtered_fls[fileprocessed:(fileprocessed+batchsize)]),function(x) parsefile(x)) -> result
  sapply(result,function(x) strsplit(grep("^Number of Axles in Train +[0-9]{1,4}",x,value=TRUE)," ")) -> trnaxleinfo
  lapply(result,function(x) strsplit(grep("^[0-9]{1,4})\t+[A-B]{1}\t",x,value=TRUE),"\t")) -> carinfo
  lapply(result,function(x) strsplit(grep("^Board: ",x,value=TRUE)," ")) -> cribinfo
  lapply(result,function(x) strsplit(grep("^Peak Axle Count: ",x,value=TRUE)," ")) -> cribaxlecount
  
  axlenumber<-c()
  for(iaxle in 1:length(trnaxleinfo))
  {
    axlenumber[iaxle]<-as.integer(trnaxleinfo[[iaxle]][length(trnaxleinfo[[iaxle]])])
  }
  
  found = FALSE
  for (ifile in 1:length(result))
  {
    icrib = 1
    trninfo<-c()
    for (iline in 1:(length(result[[ifile]])-5))
    {
      
      if(length(strsplit(result[[ifile]][iline],"\t")[[1]])==7 && found)
      {
        c(trninfo,strsplit(result[[ifile]][iline],"\t"))->trninfo
      }
      if (length(strsplit(result[[ifile]][iline],"\t")[[1]])==7 && !found)
      {
        strsplit(result[[ifile]][iline],"\t")->trninfo
        found = TRUE
      }
    }
    
    # skip_to_next <- FALSE
    # tryCatch( { buildlinearmodel(trninfo,axlenumber[ifile],carinfo[[ifile]],cribinfo[,ifile],cribaxlecount[,ifile],fls[ifile])},
    #           error = function(e){ skip_to_next <<- TRUE})
    # if(skip_to_next)
    # {
    #   next
    # }
    
    ImbalanceAnalysis(trninfo,axlenumber[ifile],carinfo[[ifile]],cribinfo[[ifile]],cribaxlecount[[ifile]],fls[fileprocessed+ifile])
    
    skip_to_next <- FALSE
    # tryCatch( {ImbalanceAnalysis(trninfo,axlenumber[ifile],carinfo[[ifile]],cribinfo[[ifile]],cribaxlecount[[ifile]],fls[fileprocessed+ifile])},
    #           error = function(e){ skip_to_next <<- TRUE})
    # if(skip_to_next)
    # {
    #   next
    # }
  }
  
  fileprocessed <- (fileprocessed+batchsize+1)
  print(fileprocessed)
  # rm(trninfo,axlenumber,carinfo,cribinfo,cribaxlecount)
}