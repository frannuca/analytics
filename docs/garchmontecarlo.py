import pandas as pd
import numpy as np

import numpy as np
from scipy import optimize
from yahoo_finance import Share

import matplotlib.pyplot as plt
from pandas.io.data import DataReader
from datetime import datetime
import random
import math
import datetime as dt


msft = DataReader("IBM","yahoo", start=datetime(2000,1,1),end=datetime(2016,01,01))     
print msft.columns

#plt.show()

prices = np.array(msft['Adj Close'])
r = []
for y in range(1,prices.size):
    r.append(math.log(prices[y])-math.log(prices[y-1]))

rtotal = np.asarray(r)
r = rtotal[:]

def GARCH11_logL(param, r):
    omega, alpha, beta = param
    n = len(r)
    s = np.ones(n)*0.01
    s[2] = np.var(r[0:3])
    for i in range(3, n):
        s[i] = omega + alpha*r[i-1]**2 + beta*(s[i-1])  # GARCH(1,1) model
    logL = -((-np.log(s) - r**2/s).sum())
    return logL


class GARCH:
   
    @staticmethod
    def sigma2(r,s2,alpha,beta,w):
      return w + alpha.transpose() * np.power(r,2) + beta.transpose() * s2;


        
    @staticmethod
    def loglikelyhoodnormal(rt,sigma2t):
        return -(np.log(sigma2t)+rt**2/sigma2t)

    @staticmethod
    def loglikelyhoodstudent_t(v,rt,s2):
        return math.gamma((v+1)/2.0)-math.gamma(v/2.0)-0.5*math.log(math.pi*(v-2))-0.5*math.log(s2)

    @staticmethod
    def calibrate(x,*args):
        
        p,q,r,v02 = args
        w0 = x[0] 
        alpha0 = np.matrix(x[1:p+1]).reshape(p,1)
        beta0  = np.matrix(x[p+1:p+q+1]).reshape(q,1)
                                          

        logval = 0
        vola2 = np.zeros((r.size,1)) + v02
        
        for t in range(max(p,q),r.size):
            sigma2t = GARCH.sigma2(np.matrix(r[t-p:t]).reshape(p,1),np.matrix(vola2[t-q:t]).reshape(q,1),alpha0,beta0,w0)
            vola2[t]=sigma2t
            logval += GARCH.loglikelyhoodnormal(r[t],sigma2t)
            #if logval != logval:
            #    print "ERROR"        

        print logval
        return -logval
        
    @staticmethod
    def createPath(xt0,w,alpha,beta,nsteps):
        dW = [random.gauss(0, 1) for _ in range(nsteps)]
       
        p = alpha.size
        q = beta.size

        a = np.matrix(alpha).reshape(p,1)
        b = np.matrix(beta).reshape(q,1)

        x = np.zeros(nsteps)
        s2 = np.zeros(nsteps)
       
        x[0]=xt0

        for n in range(max(p,q),nsteps):
            aa = w + a.transpose()*np.matrix(np.power(x[n-p:n],2)).reshape(p,1) + b.transpose()*np.matrix(s2[n-q:n]).reshape(q,1)
            s2[n] = aa
            x[n] = np.sqrt(aa)*dW[n]  
            if x[n] != x[n]:
                print "ERROR"  
         
        return x,s2
        




#initial parameters:
p=1
q=1
p0= np.array(np.random.uniform(0.01,0.1,p+q+1))
bnds = tuple((1e-12,1.0) for _ in range(1+p+q))#,(0.01,1.0),(0.01,1.0),(0.01,1.0))#,(0.01,1.0))
#cons = ({'type': 'ineq', 'fun': lambda x:  1e3*(-0.25 + sum(x[1:p+q+1]))},{'type': 'ineq', 'fun': lambda x:  1e3*(0.5 - sum(x[1:p+q+1]))})
W0 = np.var(r)
#o = optimize.minimize(fun=GARCH.calibrate,x0=p0, args=(p,q,r,W0),method='SLSQP',bounds=bnds)
o = optimize.fmin(func=GARCH.calibrate,x0=p0, args=(p,q,r,np.var(r)), full_output=1)
#R=o.x
print o
R = np.abs(o[0])
print(R)

VaR = []
NPATH = 10
NSTEPS = 100000
nvar = (int)(NSTEPS*(1-0.997))
print "NVAR="+str(nvar)

for k in range(0,NPATH):
    rx,vv = GARCH.createPath(0.0,R[0],np.matrix(R[1:p+1]).reshape(p,1),np.matrix(R[p+1:p+q+1]).reshape(q,1),NSTEPS)
    yy = np.sort(rx)[nvar]
    print "var for round "+str(k)+" is "+ str(yy)
    VaR.append(yy)

print "numerical"
print np.mean(VaR)
print "sampled total"    
print np.sort(rtotal)[(int)(rtotal.size*0.01)]
print "sampled partial"    
print np.sort(r)[(int)(r.size*0.01)]



plt.plot(rx,label='numeric returns')
plt.plot(r,label='IBM')
plt.legend()

plt.figure()
plt.plot(np.sqrt(vv),label='vola2')
plt.figure()
plt.plot(msft.index, msft['Adj Close'])
plt.show()