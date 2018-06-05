# BAMCIS AWS Elastic Transcoder Trigger
An AWS Serverless Application that launches and Elastic Transcoder job when a video file
is uploaded to S3.

## Table of Contents
- [Usage](#usage)
- [Dependencies](#dependencies)
- [Revision History](#revision-history)

## Usage
The application builds 3 S3 buckets. One bucket that the raw videos will be uploaded to, one that the transcoded video
will be saved in, and one that the transcoded video thumbnails will be saved in. It also creates a new Elastic Transcoder
Pipeline through a custom CloudFormation resource. When a raw video is uploaded to the input bucket, S3 sends an SNS
notification. The Lambda function is subscribed to that SNS topic and is triggered by that event.The function gets the 
S3 object key from the SNS message JSON text and initiates a new Elastic Transcoder job for that video in the pipeline that
was setup. Every job uses the same Preset Id that is defined in the CloudFormation parameters.

The reason this setup uses SNS as the notification configuration for S3 instead of having S3 trigger Lambda directly is the
circular reference problem that occurs between Lambda, S3, and the Lambda permissions object. Additionally, we introduce another
dependency with the Elastic Transcoder Pipeline since we need to know its Arn to supply to the Lambda function, but it can't
be known ahead of time because the Elastic Transcoder service assigns a new unique Id when the pipeline is created that is used
as part of the Arn. The pipeline also depends on the same bucket that the Lambda function does. Thus, with S3 triggering Lambda 
directly it would look like:

Bucket -> Lambda Permission -> Lambda Function -> ET Pipeline -> Bucket

Using SNS as the trigger event solves this circular dependency because the SNS topic policy doesn't require that the input
bucket or the Lambda function exist before being created, thus the SNS topic that S3 will publish to and that will trigger 
the Lambda function can be created before either of those resources.

## Dependencies

This serverless application uses a Lambda function that deploys a custom resource for the Elastic Transcoder pipeline, that
app can be found [here](https://github.com/bamcis-io/ElasticTranscoderPipeline). You will need to deploy that serverless app
in the same region you want to launch jobs to perform transcoding (i.e. the same region you deploy this serverless app) before
running this.

## Revision History

### 1.0.0
Initial release of the application.
