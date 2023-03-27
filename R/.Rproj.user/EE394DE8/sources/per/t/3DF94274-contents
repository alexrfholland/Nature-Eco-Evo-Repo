install.packages("VGAM")
library(ggplot2)
library(gridExtra)
library(dplyr)
library(VGAM)
library(viridis)


#OBSERVATION DATA
#mydata <- read.csv(file = "Birds/_2021/Data/Alex.csv")

mydata <- read.csv(file = "Data/observed bird-branch interactions.csv")



names(mydata) #lists the variable names as written by R - note that these differ from how they are written in Excel

mydata$Branch.type=as.factor(mydata$Branch.type)

summary(mydata)

#_____________________________________________________________________________
#Model for predicting numbers of birds
library(VGAM)
glmm4 <- vglm(number.of.individual.birds ~ DBH + Branch.type + Branch.angle, family = pospoisson(), data = mydata)

glmm4 <- vglm(number.of.individual.birds ~ DBH + Branch.type + Branch.angle, family = pospoisson(), data = mydata)


fitted(glmm4)



#__________________________________________________________________
## Plot predictions and CIs for individual birds model

newdata <- expand.grid(DBH=30:170, Branch.angle=10, Branch.type=c("dead","live"))

## function to return predicted values
fpred <- function(mydata, i, newdata) {
  require(VGAM)
  m <- vglm(number.of.individual.birds ~ DBH + Branch.type + Branch.angle, family = pospoisson(),
            data = mydata[i, ])
  predict(m, newdata, type = "response")
}

## set seed and run bootstrap with 1,200 draws
library(boot)
set.seed(10)
respred <- boot(mydata, fpred, R = 1200, newdata = newdata,
                parallel = "snow", ncpus = 4)

## get the bootstrapped percentile CIs
yhat <- t(sapply(1:nrow(newdata), function(i) {
  out <- boot.ci(respred, index = i, type = c("perc"))
  with(out, c(Est = t0, pLL = percent[4], pUL = percent[5]))
}))

## merge CIs with predicted values
newdata <- cbind(newdata, yhat)



newdata2 <- expand.grid(DBH=82, Branch.angle=0:90, Branch.type=c("dead","live"))

## function to return predicted values
fpred <- function(mydata, i, newdata2) {
  require(VGAM)
  m <- vglm(number.of.individual.birds ~ DBH + Branch.type + Branch.angle, family = pospoisson(),
            data = mydata[i, ])
  predict(m, newdata2, type = "response")
}

## set seed and run bootstrap with 1,200 draws
library(boot)
set.seed(10)
respred <- boot(mydata, fpred, R = 1200, newdata = newdata2,
                parallel = "snow", ncpus = 4)

## get the bootstrapped percentile CIs
yhat <- t(sapply(1:nrow(newdata2), function(i) {
  out <- boot.ci(respred, index = i, type = c("perc"))
  with(out, c(Est = t0, pLL = percent[4], pUL = percent[5]))
}))

## merge CIs with predicted values
newdata2 <- cbind(newdata2, yhat)


individuals1 <- ggplot(newdata, aes(x = DBH, y = Est, colour = Branch.type, fill = Branch.type)) +
  geom_ribbon(aes(ymin = pLL, ymax = pUL), alpha = .25) +
  geom_smooth() +
  ylab("Number of individual birds") +
  xlab("Tree diameter (cm)") +
  ylim(1,2.55) +
  theme(legend.position="none")


individuals2 <- ggplot(newdata2, aes(x = Branch.angle, y = Est, colour = Branch.type, fill = Branch.type)) +
  geom_ribbon(aes(ymin = pLL, ymax = pUL), alpha = .25) +
  geom_smooth() +
  ylab("Number of individual birds") +
  xlab("Branch angle (degrees from horizontal)") +
  ylim(1,2.55) +
  theme(legend.position = c(0.5, 0.90),legend.direction = "vertical")

arrange <- grid.arrange(individuals1, individuals2, nrow=1)
ggsave("Fig1.tiff", arrange, dpi=300)


##updated style
library(hrbrthemes)

opt = "A"
top = .8
bot = 0.1
e = 0.8

individuals1 <- ggplot(newdata, aes(x = DBH, y = Est, colour = Branch.type, fill = Branch.type)) +
  geom_ribbon(aes(ymin = pLL, ymax = pUL), alpha = .25) +
  geom_smooth() +
  ylab("Number of individual birds") +
  xlab("Tree trunk diameter at breast height (cm)") +
  ylim(1,2.55) +
  theme_ipsum(ticks = TRUE, axis_text_size = 9.5, axis_title_size = 12, plot_title_size = 14
  ) +
  theme(
    panel.spacing = unit(2, "lines"),
    panel.grid.major = element_blank(),
    panel.grid.minor = element_blank(),
    panel.border = element_rect(fill=NA, size=.5),
    panel.background = element_blank(),
    legend.position = "bottom", legend.title =element_text(size=12),
    legend.text= element_text(size=9.5),
    legend.justification = c(0, 1),
    axis.ticks.length = unit(0.4, "cm")) +
  #scale_color_grey(start = top, end = bot, name = "Branch State", labels = c("Dead", "Live")) +
  #scale_fill_grey(start = top, end = top, name = "Branch State", labels = c("Dead", "Live"))
  scale_color_viridis(option = opt, discrete = TRUE, name = "Branch State", labels = c("Dead", "Live"), end = e) +
  scale_fill_viridis(option = opt, discrete = TRUE, name = "Branch State", labels = c("Dead", "Live"), end = e)


individuals2 <- ggplot(newdata2, aes(x = Branch.angle, y = Est, colour = Branch.type, fill = Branch.type)) +
  geom_ribbon(aes(ymin = pLL, ymax = pUL), alpha = .25) +
  geom_smooth() +
  ylab("Number of individual birds") +
  xlab("Branch angle (degrees from horizontal)") +
  ylim(1,2.55) +
  theme(legend.position = c(0.5, 0.90),legend.direction = "vertical") +
  theme_ipsum(ticks = TRUE, axis_text_size = 9.5, axis_title_size = 12, plot_title_size = 14
  ) +
  theme(
    panel.spacing = unit(2, "lines"),
    panel.grid.major = element_blank(),
    panel.grid.minor = element_blank(),
    panel.border = element_rect(fill=NA, size=.5),
    panel.background = element_blank(),
    legend.position = "bottom", legend.title =element_text(size=12),
    legend.text= element_text(size=9.5),
    legend.justification = c(0, 1),
    axis.ticks.length = unit(0.4, "cm")) +
  #scale_color_grey(start = top, end = bot, name = "Branch State", labels = c("Dead", "Live")) +
  #scale_fill_grey(start = top, end = top, name = "Branch State", labels = c("Dead", "Live"))
    scale_color_viridis(option = opt, discrete = TRUE, name = "Branch State", labels = c("Dead", "Live"), end = e) +
    scale_fill_viridis(option = opt, discrete = TRUE, name = "Branch State", labels = c("Dead", "Live"), end = e)
  

arrange <- grid.arrange(individuals1, individuals2, nrow=1)


pred = ggplot(branchsWithPred, aes(fill=factor(suitInd), y=length, x=factor(Tree))) + 
  geom_bar(position="dodge", stat="identity", width = wi) +
  scale_y_continuous(trans=scales::pseudo_log_trans(base = 10), breaks=c(10,100,1000,2000)) +
  scale_x_discrete(labels = realDBHs) +
  scale_fill_viridis(discrete = TRUE, name = "Predicted Branch Suitability", labels = c("Not Suitable", "Minimally Suitable", "Suitable")) +
  theme_ipsum(ticks = TRUE, axis_text_size = 9.5, axis_title_size = 12, plot_title_size = 14
  ) +
  theme(
    panel.spacing = unit(2, "lines"),
    panel.grid.major = element_blank(),
    panel.grid.minor = element_blank(),
    panel.border = element_rect(fill=NA, size=.5),
    panel.background = element_blank(),
    legend.position = "bottom", legend.title =element_text(size=12),
    legend.text= element_text(size=9.5),
    legend.justification = c(0, 1),
    axis.ticks.length = unit(0.4, "cm")
  ) +
  geom_errorbar(aes(ymin=length.low, ymax=length.high, color = factor(suitInd)),
                size=.8,    # Thinner lines
                width=0,
                position=position_dodge(wi)) +
  scale_color_viridis(discrete = TRUE, name = "Predicted Branch Suitability", labels = c("Not Suitable", "Minimally Suitable", "Suitable")) +
  ggtitle("Predicted Availability of Suitable Branch Structures Across Trees") +
  ylab("Total branch length (m)") + xlab("Tree Diameter at Breast Height (cm)")

pred



#______________________________________________________________________________

#This code provides predicted number of birds and the lower and upper 95% confidence intervals for each branch from your scans
#nd <- read.table("E:/R/Alex/branchdata.csv", header=T, sep=",")

#nd <- read.csv(file = "Birds/_2021/Data/branchData2.csv")

nd <- read.csv(file = "Data/detected branch features.csv.csv")

newdata <- data.frame(nd)
names(newdata)

## function to return predicted values
fpred <- function(mydata, i, newdata) {
  require(VGAM)
  m <- vglm(number.of.individual.birds ~ DBH + Branch.type + Branch.angle, family = pospoisson(),
            data = mydata[i, ], coefstart = c(-1.168, 0.010, -0.972, -0.018))
  predict(m, newdata, type = "response")
}

#updated
fpred <- function(mydata, i, newdata) {
  require(VGAM)
  m <- vglm(number.of.individual.birds ~ DBH + Branch.type + Branch.angle, family = pospoisson(),
            data = mydata[i, ], coefstart = c(-1.168, 0.010, -0.972, -0.018))
  predict(m, newdata, type = "response")
}

## set seed and run bootstrap with 1,200 draws
library(boot)
set.seed(10)
respred <- boot(mydata, fpred, R = 1200, newdata = newdata,
                parallel = "snow", ncpus = 4)

## get the bootstrapped percentile CIs
yhat <- t(sapply(1:nrow(newdata), function(i) {
  out <- boot.ci(respred, index = i, type = c("perc"))
  with(out, c(Est = t0, pLL = percent[4], pUL = percent[5]))
}))

## merge CIs with predicted values
newdata <- cbind(newdata, yhat)
print(newdata)

write.csv(newdata,"Birds/_2022 - Scanning/Outputs/updatedData", row.names = T)







#________________________________________________________________________
#Model for bird species richness


glmm8 <- vglm(number.of.bird.species ~ DBH + Branch.type + Branch.angle, family = pospoisson(), data = mydata)
fitted(glmm8)
summary(glmm8)



#__________________________________________________________________
## Plot predictions and CIs for species model

newdata <- expand.grid(DBH=30:170, Branch.angle=10, Branch.type=c("dead","live"))

## function to return predicted values
fpred <- function(mydata, i, newdata) {
  require(VGAM)
  m <- vglm(number.of.bird.species ~ DBH + Branch.type + Branch.angle, family = pospoisson(),
            data = mydata[i, ])
  predict(m, newdata, type = "response")
}

## set seed and run bootstrap with 1,200 draws
library(boot)
set.seed(10)
respred <- boot(mydata, fpred, R = 1200, newdata = newdata,
                parallel = "snow", ncpus = 4)

## get the bootstrapped percentile CIs
yhat <- t(sapply(1:nrow(newdata), function(i) {
  out <- boot.ci(respred, index = i, type = c("perc"))
  with(out, c(Est = t0, pLL = percent[4], pUL = percent[5]))
}))

## merge CIs with predicted values
newdata <- cbind(newdata, yhat)



newdata2 <- expand.grid(DBH=82, Branch.angle=0:90, Branch.type=c("dead","live"))

## function to return predicted values
fpred <- function(mydata, i, newdata2) {
  require(VGAM)
  m <- vglm(number.of.bird.species ~ DBH + Branch.type + Branch.angle, family = pospoisson(),
            data = mydata[i, ])
  predict(m, newdata2, type = "response")
}

## set seed and run bootstrap with 1,200 draws
library(boot)
set.seed(10)
respred <- boot(mydata, fpred, R = 1200, newdata = newdata2,
                parallel = "snow", ncpus = 4)

## get the bootstrapped percentile CIs
yhat <- t(sapply(1:nrow(newdata2), function(i) {
  out <- boot.ci(respred, index = i, type = c("perc"))
  with(out, c(Est = t0, pLL = percent[4], pUL = percent[5]))
}))

## merge CIs with predicted values
newdata2 <- cbind(newdata2, yhat)


species1 <- ggplot(newdata, aes(x = DBH, y = Est, colour = Branch.type, fill = Branch.type)) +
  geom_ribbon(aes(ymin = pLL, ymax = pUL), alpha = .25) +
  geom_smooth() +
  ylab("Number of bird species") +
  xlab("Tree diameter (cm)") +
  ylim(1,2.2) +
  theme(legend.position="none")


species2 <- ggplot(newdata2, aes(x = Branch.angle, y = Est, colour = Branch.type, fill = Branch.type)) +
  geom_ribbon(aes(ymin = pLL, ymax = pUL), alpha = .25) +
  geom_smooth() +
  ylab("Number of bird species") +
  xlab("Branch angle (degrees from horizontal)") +
  ylim(1,2.2) +
  theme(legend.position = c(0.5, 0.90),legend.direction = "vertical")

arrange <- grid.arrange(species1, species2, nrow=1)
ggsave("Fig2.tiff", arrange, dpi=300)



#________________________________________________________________________________
#This code provides predicted number of birds and the lower and upper 95% confidence intervals for each branch from your scans
nd <- read.table("Birds/_2022 - Scanning/Data/updatedData.csv", header=T, sep=",")
newdata <- data.frame(nd)
names(newdata)

names(mydata)

## function to return predicted values - species
fpred <- function(mydata, i, newdata) {
  require(VGAM)
  m <- vglm(number.of.bird.species ~ DBH + Branch.type + Branch.angle, family = pospoisson(),
            data = mydata[i, ], coefstart = c(-2.888, 0.015, -0.386, -0.030))
  predict(m, newdata, type = "response")
}


## function to return predicted values - individuals. Check?
fpred <- function(mydata, i, newdata) {
  require(VGAM)
  m <- vglm(number.of.individual.birds ~ DBH + Branch.type + Branch.angle, family = pospoisson(),
            data = mydata[i, ], coefstart = c(-2.888, 0.015, -0.386, -0.030))
  predict(m, newdata, type = "response")
}


## set seed and run bootstrap with 1,200 draws
library(boot)
set.seed(10)
respred <- boot(mydata, fpred, R = 1200, newdata = newdata,
                parallel = "snow", ncpus = 4)

## get the bootstrapped percentile CIs
yhat <- t(sapply(1:nrow(newdata), function(i) {
  out <- boot.ci(respred, index = i, type = c("perc"))
  with(out, c(Est = t0, pLL = percent[4], pUL = percent[5]))
}))

## merge CIs with predicted values
newdata <- cbind(newdata, yhat)
print(newdata)

write.csv(newdata, "Birds/_2022 - Scanning/Outputs/Processing/branchPredictions - species.csv", row.names = T)
write.csv(newdata, "Birds/_2022 - Scanning/Outputs/Processing/branchPredictions - individuals.csv", row.names = T)


##renaame individual and species predictions

inds <- read.csv(file = "Birds/_2022 - Scanning/Outputs/Processing/branchPredictions - individuals.csv")
spei <- read.csv(file = "Birds/_2022 - Scanning/Outputs/Processing/branchPredictions - species.csv")


spei = spei %>%
  rename(speEst = Est,
         spepLL = pLL,
         spepUL = pUL
  )
  
inds = inds %>%
  rename(indEst = Est,
         indpLL = pLL,
         indpUL = pUL
  )

##merge  individual and species predictions

speiExtract = spei %>%
  select(speEst, spepLL, spepUL)
branchPredictions = bind_cols(inds, speiExtract)

write.csv(branchPredictions, "Birds/_2022 - Scanning/Outputs/branchPredictions - full.csv", row.names = T)