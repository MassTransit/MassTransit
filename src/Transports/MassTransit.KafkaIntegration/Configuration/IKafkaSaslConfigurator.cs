namespace MassTransit
{
    using Confluent.Kafka;


    public interface IKafkaSaslConfigurator
    {
        /// <summary>
        /// SASL mechanism to use for authentication. Supported: GSSAPI, PLAIN, SCRAM-SHA-256, SCRAM-SHA-512. **NOTE**: Despite the name, you may not configure more than one mechanism.
        /// </summary>
        SaslMechanism? Mechanism { set; }

        /// <summary>
        /// Kerberos principal name that Kafka runs as, not including /hostname@REALM
        /// default: kafka
        /// importance: low
        /// </summary>
        string KerberosServiceName { set; }

        /// <summary>
        /// This client's Kerberos principal name. (Not supported on Windows, will use the logon user's principal).
        /// default: kafkaclient
        /// importance: low
        /// </summary>
        string KerberosPrincipal { set; }

        /// <summary>
        /// Shell command to refresh or acquire the client's Kerberos ticket. This command is executed on client creation and every sasl.kerberos.min.time.before.relogin (0=disable).
        /// %{config.prop.name} is replaced by corresponding config object value.
        /// default: kinit -R -t "%{sasl.kerberos.keytab}" -k %{sasl.kerberos.principal} || kinit -t "%{sasl.kerberos.keytab}" -k %{sasl.kerberos.principal}
        /// importance: low
        /// </summary>
        string KerberosKinitCmd { set; }

        /// <summary>
        /// Path to Kerberos keytab file. This configuration property is only used as a variable in `sasl.kerberos.kinit.cmd` as ` ... -t "%{sasl.kerberos.keytab}"`.
        /// default: ''
        /// importance: low
        /// </summary>
        string KerberosKeytab { set; }

        /// <summary>
        /// Minimum time in milliseconds between key refresh attempts. Disable automatic key refresh by setting this property to 0.
        /// default: 60000
        /// importance: low
        /// </summary>
        int? KerberosMinTimeBeforeRelogin { set; }

        /// <summary>
        /// SASL username for use with the PLAIN and SASL-SCRAM-.. mechanisms
        /// default: ''
        /// importance: high
        /// </summary>
        string Username { set; }

        /// <summary>
        /// SASL password for use with the PLAIN and SASL-SCRAM-.. mechanism
        /// default: ''
        /// importance: high
        /// </summary>
        string Password { set; }

        /// <summary>
        /// SASL/OAUTHBEARER configuration. The format is implementation-dependent and must be parsed accordingly. The default unsecured token implementation (see
        /// https://tools.ietf.org/html/rfc7515#appendix-A.5) recognizes space-separated name=value pairs with valid names including principalClaimName, principal, scopeClaimName, scope,
        /// and lifeSeconds. The default value for principalClaimName is "sub", the default value for scopeClaimName is "scope", and the default value for lifeSeconds is 3600. The scope
        /// value is CSV format with the default value being no/empty scope. For example: `principalClaimName=azp principal=admin scopeClaimName=roles scope=role1,role2 lifeSeconds=600`.
        /// In addition, SASL extensions can be communicated to the broker via `extension_NAME=value`. For example: `principal=admin extension_traceId=123`
        /// default: ''
        /// importance: low
        /// </summary>
        string OauthbearerConfig { set; }

        /// <summary>
        /// Enable the builtin unsecure JWT OAUTHBEARER token handler if no oauthbearer_refresh_cb has been set. This builtin handler should only be used for development or testing, and
        /// not in production.
        /// default: false
        /// importance: low
        /// </summary>
        bool? EnableOauthbearerUnsecureJwt { set; }

        /// <summary>
        /// Set the "security.protocol" property on the client configuration
        /// default: null
        /// importance: low
        /// </summary>
        SecurityProtocol? SecurityProtocol { set; }
    }
}
