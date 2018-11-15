#!groovy

/*
	=======================
	APPLIED ABC CI PIPELINE
	=======================
	
	--------------
	CONIFGURATIONS
	--------------
	Various configurations available within this jenkinsfile.
	The configurations are run depending on the Jenkins job that
	invokes this.  For example, one job might be set up on pushes to
	the branch, another configured to run nightly, and another to
	run on-demand.  This file uses the job's name to determine which
	configuration to run.
		
	Configurations are 'stacked': 
	
	  Unit Testing:			runs only unit tests
	  Integration Testing: 	runs unit and integration tests
	  System Testing: 		runs unit, integration and system tests
	  UI Testing: 			runs unit, integration, system and UI tests
	
	Any Deploy configuration will run all tests prior to deployment
	
	
	
	
*/

node {

	currentBuild.result = "SUCCESS"	

	checkout scm
	
	if (JOB_BASE_NAME == 'AABC_Develop_Tests_Unit') 
	{ 
		echo 'RUNNING AABC_Develop_Tests_Unit configuration...'
		try 
		{
			buildUnitTests()
			runUnitTests()
			currentBuild.result = "SUCCESS"
		} 
		catch (err) 
		{
			currentBuild.result = "FAILURE"
		} 
		finally 
		{
			cleanup()
		}
	}
	
	if (JOB_BASE_NAME == 'AABC_Develop_Tests_Integration') 
	{
		echo 'RUNNING AABC_Develop_Tests_Integration configuration...'
		buildIntegrationTests()
		runIntegrationTests()
		cleanup()
	}
	
	if (JOB_BASE_NAME == 'AABC_Develop_Tests_System')
	{
		echo 'RUNNING AABC_Develop_Tests_System configuration...'
	}
	
	
	
}	// end node


def buildUnitTests() 
{	
	stage('Build') 
	{			
		bat 'CI/build.solution.aabc.web'
	}	
}	

def runUnitTests() 
{
	stage('Test') 
	{
		bat 'CI/tests.unit.aabc.web'
	}
}	

def cleanup() 
{
	stage('Cleanup') 
	{
		mail body: "${JOB_BASE_NAME} ${currentBuild.result} - See details here: ${BUILD_URL}" ,
			from: "jenkins@dymeng.com",
			to: "jleach@dymeng.com",
			subject: "${currentBuild.result} - ${JOB_BASE_NAME} "
	}
}
