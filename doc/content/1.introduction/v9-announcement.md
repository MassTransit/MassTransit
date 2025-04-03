# Announcing MassTransit v9

## A New Era for Enterprise Messaging

For over a decade, MassTransit has been the leading open-source .NET messaging framework, trusted by thousands developers and enterprises worldwide to build
scalable, distributed applications. It has facilitated billions of transactions, powered mission-critical systems in finance, healthcare, government, and
logistics, and seamlessly integrated with cloud platforms like Microsoft Azure, AWS, and Google Cloud. As adoption has surged and enterprise demand has grown,
MassTransit has evolved into a cornerstone of modern event-driven architectures. Today, we’re excited to share an important milestone!

::alert{type="info"}
Update - post announcement Q&A added [below](#qa) (updated again, 2025-04-03 9:30AM CDT)
::

### We are transitioning to a commercial model.

With MassTransit v9, we are transitioning to a commercial licensing model. This change ensures long-term sustainability, continued innovation, and
enterprise-grade support — while leaving MassTransit v8 open-source and available to the community.

## Why the Change?

When MassTransit first started in 2007, it was a single assembly that supported MSMQ. Fast-forward to today, and MassTransit has extensive support for several
message brokers, multiple databases, powerful capabilities including saga state machines, job consumers, message scheduling, and routing slips, as well as its
own SQL-based message transport. The entire solution builds and deploys over thirty NuGet packages.

Because of its extensive capabilities, MassTransit has grown into a mission-critical foundation for organizations worldwide. Trusted by enterprises in more than
100 countries across industries including finance, healthcare, logistics, and government. As adoption has surged, so has the need for:

- Dedicated, full-time development resources to enhance and maintain the platform.
- Enterprise-grade support and guarantees for business-critical applications.
- A sustainable, long-term funding model to drive continuous innovation.

By making v9 a commercial release, we can accelerate MassTransit’s evolution, delivering even better scalability, security, and performance — while continuing
to support the existing community.

### Change is Hard, Why Now?

We know that change — especially in the open-source world — can be challenging. If you’ve been relying on MassTransit as a free and open solution, you might be
wondering what this shift means for you and your team. We want to be clear: we deeply appreciate the community that has helped make MassTransit what it is
today. This decision wasn’t made lightly, but rather with the long-term success and sustainability of the project in mind.

By moving to a commercial model for v9, we’re ensuring that MassTransit can continue to evolve, with dedicated resources, faster development cycles, and the
enterprise support that many organizations have been asking for. MassTransit v8 remains open-source, and we’re committed to making this transition as smooth
as possible. Our goal isn’t to take something away — but to build something even better, with the stability and support that businesses need. We’re here to
help, and we welcome your questions, feedback, and thoughts as we move forward together.

## What This Means for You

### MassTransit v8 Remains Open Source

- The existing v8 codebase will remain open-source and available under its current license.
- Security patches and critical bug fixes will continue for a transition period.
- Community support remains available for v8 users.

### MassTransit v9 Becomes a Commercial Product

- New features, performance enhancements, and enterprise-focused capabilities will be exclusive to v9.
- Available via a commercial license with support plans tailored for organizations of all sizes, including rates for independent software vendors and
  consultants building and
  supporting their own customer applications using MassTransit.
- Expert assistance, SLAs, and long-term stability guarantees for enterprise users.

## The Transition Plan

Moving to a commercial product is a huge undertaking, and the initial target dates for the transition are outlined below (and are subject to change).

| Date           | Milestone                                                       |
|----------------|:----------------------------------------------------------------|
| **Q3 2025**    | MassTransit v9 prerelease packages available to early adopters. |
| **Q1 2026**    | MassTransit v9 official release under a commercial license.     |
| **Ongoing**    | MassTransit v8 security patches and community support continue. |
| **After 2026** | End of official maintenance for MassTransit v8.                 |

## How Do I Prepare?

If you're currently using MassTransit v8:

- You can continue using v8 with no changes — it's open-source and will remain available.
- If you need commercial support, [support agreements](/support) are available.
- If you want access to v9 and future innovations, consider our commercial licensing options (to be announced at a later date).

If you currently have a support agreement:

- You will be granted a license for MassTransit v9 through the remainder of your support period.
- Support will continue for your existing MassTransit v8 applications.

## Q&amp;A

### How much will it cost?

Understandably the most asked question, and with good reason. Pricing will be reasonable and split into small/medium-sized businesses and large organizations,
and subscription-based. The best pricing will be for annual subscriptions (equivalent to ten monthly payments), but monthly subscriptions will be available as
well.

The price target for small/medium-sized business is $400 USD/month, or $4000 USD/year (direct billed, payable via invoice, bank transfer or credit card). For
large organizations with multiple teams or complex "enterprise-level" procurement policies, the price target is closer to $1200 USD/month, or $12000 USD/year.

_We will consider a top-line revenue threshold so that small and pre-revenue startup companies can use MassTransit v9 at a significantly reduced cost (similar to
what other solutions have adopted)_.

These price targets would include standard email-based support with a 72-hour response time. Shorter SLA's may be available, but with a price surcharge.

There will be no cost to install the packages and use them for local development purposes (evaluation), a temporary license will be generated and the software
will run unlicensed for a short evaluation period.

There are no plans at this time to offer any sort of "ala carte" pricing for individual components, transports, etc.

::alert{type="warning"}
To be clear, these are price targets, not finalized prices. It's important to set expectations early rather than encourage speculation. And we will continue to
listen to feedback and make adjustments.
::

### What about consultants/agencies using MT in customer solutions?

Developers have helped make MassTransit the great success it is today. And we want to continue to encourage its use when building customer applications.

If you think about it, building a customer application that requires messaging ultimately need to use a message broker. And if your team is going to code to the
native APIs, your customer is paying you money to write code that is _not_ business value. It's a "how hard can it be" moment, when the details come back to
bite you. It's why many consulting companies use MassTransit – reduced time to value. Developers focus on business value, not infrastructure.

To that end, the goal is to provide a way for consultants to build and deploy applications using MassTransit without forcing customers to understand and manage
the license. We'll work through the details, but the general structure would be an ISV could license MassTransit, and deploy client applications with a valid
license. Since license checks are tied to the deployment (vs an expiration date), the software would continue to run without interruption. Updates to that
application would require a valid license at the time of the update's deployment.

The details aren't ironed out yet, but we're open to feedback on how to work with both small (1-2 person shops) and large (1mil, 10+mil) agencies to ensure that
license management is simple and straightforward.

### Will there be a non-commercial license for v9?

As stated above, the transition plan includes ongoing patches and updates for v8. Developers can continue to use v8 during the transition, and won't be _forced_
to upgrade to v9. To take advantage of new features and enhancements, developers would need to upgrade to the licensed version.

Patches and updates to v8 through at least the end of 2026. That's 1.75 years from now, giving developers plenty of runway to plan their migration to v9.
*That's longer than the support window for some .NET versions!*

### Will there be a free license for non-profits?

Applications for a non-profit license may be considered on a case-by-case basis, but not initially. Details about special pricing for not-for-profit
organizations may be announced at a future date. As stated above, v8 will remain open source and continue to receive patches/updates at least through 2026.

### Will licensing be complex, seat-based, etc.?

MassTransit will use a simple license structure as outlined in the "How much will it cost" answer above. Customers will be provided a license file that must be
deployed with the application.

> The concept of dynamic license updates has been removed, the license will be valid for the deployment without expiration.

### What happens if our license expires?

The license deployed with the application is bound to that application version at deployment time. Once deployed, the license will remain valid. If the
application is updated, the new deployment would require a current, valid license to deploy the update. This ensures that applications built and deployed to
production environments will continue to run without interruption or degradation.

> Note, this redacts previous statements about run-time license enforcement, which after careful consideration from community feedback has been dropped.

### Will the v9 code be open source?

Probably. I expect tha the v9 code will become the main branch in the existing repository, with v8 being a separate branch/codebase. NuGet packages will be
built and deployed as they are today, via [NuGet](https://www.nuget.org/packages/MassTransit/). To ease upgrades, the packages will be published with the same
names, namespaces, etc. so that applications will compiled in v9 the same as they did in v8. At least, that's the way I see it right now.

### Did you coordinate with Jimmy to break the news?

**NO**, it is just a coincidence that we both chose _not_ to announce our plans on April 1st.

## Next Steps

We understand this is a significant transition, and we’re committed to making it as smooth as possible. If you have questions about licensing, support, or
migration options:

**[Contact Us](mailto:support@masstransit.io)**

Thank you for being a valued MassTransit user. We’re excited for the future and look forward to continuing to support your success!  
