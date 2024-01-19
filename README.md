# Serverless User API

The Serverless User API is a .NET 6.0-based project utilizing AWS Lambda, AWS DynamoDB, Simple Notification Service (SNS), AWS Rekognition, and AI for user verification. This API is designed for storing and managing user personal data with extendable features. It supports user profile suspension, deletion, adding mobile devices for push notifications, user verification via ID card or password and selfie, adding avatars, reporting user profiles, and real-time data changes using AWS SNS.

## Infrastructure Diagram
![user-api-infra.png](docs%2Fuser-api-infra.png)

## Prerequisites

Before you begin, ensure you have the following prerequisites:

- **AWS Account:**
  - You'll need an AWS account with appropriate permissions to deploy and manage AWS services used in this project.

- **.NET 6.0 SDK:**
  - Install the [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) on your local machine.

- **AWS Services Knowledge:**
  - Basic knowledge of AWS services, including Lambda, API Gateway, and DynamoDB, is recommended.

- **Familiarity with Serverless Architecture:**
  - Understanding serverless architecture concepts will help you make the most of this project.

## Features

- **User Personal Data:**
  - Store and operate user personal data in a scalable manner.

- **Extendability:**
  - The API is designed to be easily extendable for future feature additions.

- **Profile Management:**
  - Suspend and delete user profiles as needed.

- **Push Notifications:**
  - Add user mobile devices to receive push notifications.

- **User Verification with AI:**
  - Verify users using AI technology for enhanced security (utilizes AWS Rekognition for facial recognition and identity verification).

- **Avatar Support:**
  - Allow users to add avatars to their profiles.

- **Reporting:**
  - Provide functionality for reporting user profiles.

- **Real-time Data Changes:**
  - Publish messages to AWS Simple Notification Service (SNS) when user data changes.
    - Consumers can subscribe to the SNS topic to receive notifications about user data modifications.
    - 
## Usage
The Serverless User API provides the following endpoints:

![api-referance.png](docs%2Fapi-referance.png)

## Getting Started

To get started with the Serverless User API, perform the following steps:

1. Set up an AWS account if you don't already have one.
2. Install the .NET 6.0 SDK on your development machine.
3. Clone the Serverless User API repository: [https://github.com/fehmianac/serverless-user-api]([https://github.com/your-username/your-repo](https://github.com/fehmianac/serverless-user-api))
4. Navigate to the repository's root directory.
5. Deploy the CloudFormation stack using the template.yaml file. This will create the necessary AWS resources.
6. Once the stack is deployed successfully, you can start using the Serverless User API.

## Contributions

Contributions to the Serverless User API are welcome! If you find any issues or want to suggest improvements, please submit an issue or pull request on the [GitHub repository](https://github.com/fehmianac/serverless-user-api).

## Contact Information

For any inquiries or assistance, please contact Fehmi Anaç at fehmianac@gmail.com.
